<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:denina="http://denina"
>
    <xsl:output method="html" indent="yes"/>


  <xsl:template match="category">
    <div class="denina-category" id="denina-category-{denina:CleanFileName(categoryMeta/Category)}">
      <xsl:apply-templates select="categoryMeta/Category"/>
      <xsl:apply-templates select="categoryMeta/Description"/>
      <xsl:apply-templates select="filter">
        <xsl:sort select="filterMeta/Name"/>
      </xsl:apply-templates>
    </div>
  </xsl:template>

  <!-- Categories -->
  <xsl:template match="categoryMeta/Category">
    <h1>
      <xsl:value-of select="."/>
    </h1>
  </xsl:template>

  <xsl:template match="categoryMeta/Description">
    <p>
      <xsl:value-of select="."/>
    </p>
  </xsl:template>
  
  <!-- Filters -->
  <xsl:template match="filter">
    <div class="denina-filter" id="denina-filter-{denina:CleanFileName(filterMeta/Name)}">
      <xsl:apply-templates select="filterMeta/Name"/>
      <xsl:apply-templates select="filterMeta/Description"/>
      <xsl:apply-templates select="arguments"/>
      <xsl:apply-templates select="samples"/>
    </div>
  </xsl:template>
  
  <xsl:template match="filterMeta/Name">
    <h2>
      <xsl:value-of select="."/>
    </h2>    
  </xsl:template>

  <xsl:template match="filterMeta/Description">
    <p>
      <xsl:value-of select="."/>
    </p>
  </xsl:template>

  <xsl:template match="arguments">
    <h3>Arguments</h3>
    <table class="denina-arguments">
      <colgroup>
        <col class="pos"/>
        <col class="name"/>
        <col class="req"/>
        <col class="desc"/>
      </colgroup>
      <tr>
        <th>Pos</th>
        <th>Name</th>
        <th>Required</th>
        <th>Description</th>
      </tr>
      <xsl:apply-templates>
        <xsl:sort select="argumentMeta/Ordinal" data-type="number"/>
      </xsl:apply-templates>
    </table>
  </xsl:template>

  <xsl:template match="samples">
    <h3>Samples</h3>
    <xsl:apply-templates/>
  </xsl:template>

  
  <!-- Arguments -->
  <xsl:template match="argumentMeta">
    <tr>
      <xsl:apply-templates select="Ordinal"/>
      <xsl:apply-templates select="Name"/>
      <xsl:apply-templates select="Required"/>
      <xsl:apply-templates select="Description"/>
    </tr>
  </xsl:template>
 
  <xsl:template match="argumentMeta/*">
    <td>
      <xsl:value-of select="."/>
    </td>
  </xsl:template>

  <!-- Samples -->
  <xsl:template match="codeSample">
    <table class="denina-samples">
      <tr class="input">
        <th>Input</th>
        <xsl:apply-templates select="Input"/>
      </tr>
      <tr class="command">
        <th>Command</th>
        <xsl:apply-templates select="Command"/>
      </tr>
      <tr class="output">
        <th>Output</th>
        <xsl:apply-templates select="Output"/>
      </tr>
    </table>
  </xsl:template>

  <xsl:template match="codeSample/*">
    <td>
      <code>
        <xsl:copy-of select="denina:NewLines(.)"/>
      </code>
    </td>
  </xsl:template>

  <xsl:template match="codeSample/Input|codeSample/Output">
    <td>
          <xsl:choose>
            <xsl:when test="substring(.,1,1) = '(' and substring(.,string-length(.),1) = ')'">
              <xsl:value-of select="substring(.,2,string-length(.) - 2)"/>
            </xsl:when>
            <xsl:when test="string-length(.) = 0">
              
            </xsl:when>
            <xsl:otherwise>
              <code>
                <xsl:copy-of select="denina:NewLines(.)"/>
              </code>
            </xsl:otherwise>
          </xsl:choose>
    </td>
  </xsl:template>

  
  
  <xsl:template match="text()">
    
  </xsl:template>


</xsl:stylesheet>
