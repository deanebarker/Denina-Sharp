<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:denina="http://denina"
>
    <xsl:output method="html" indent="yes"/>

  <xsl:template match="/">
    <div id="denina-home">
      <h1>Denina Command Reference</h1>
      <p>
        Denina filters are grouped into categories.
      </p>
      <ul class="denina-toc">
        <xsl:apply-templates select="//category">
          <xsl:sort select="categoryMeta/Category" data-type="text"/>
        </xsl:apply-templates>
      </ul>
    </div>
  </xsl:template>
  
  <xsl:template match="category">
    <li>
      <xsl:apply-templates select="categoryMeta/Category"/>
      <xsl:apply-templates select="categoryMeta/Description"/>
    </li>
  </xsl:template>

  <xsl:template match="categoryMeta/Category">
    <a>
      <xsl:attribute name="href"><xsl:value-of select="denina:CleanFileName(.)"/>.html</xsl:attribute>
      <xsl:value-of select="."/>
    </a>
  </xsl:template>

  <xsl:template match="categoryMeta/Description">
    <xsl:value-of select="."/>
  </xsl:template>
</xsl:stylesheet>
