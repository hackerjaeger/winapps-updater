﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "109.0b3";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c38b69a8b6677314b86aba713ddfda0a8bb8edff40af56b259f451d3c5d248c66c087340275aae391afbf57cad53c6440d6d76dc34fc521ac3cc9b8324c57f17" },
                { "af", "485864d5bad0d2171f48de7535bfd6df81abfc2d893b5ad1e167da44e9aba830df4c3cde99cbbea1e47e5c42d9724756a47dc10a60beeea34affad6a537348ef" },
                { "an", "67cff7031253d259c98ea365db06ecc876168cf1ac255709abe989b35c82faa9d4913a283415e72a4204775c017d45d16c4349d50d39a52885786ccabf2b6da3" },
                { "ar", "4eecd6f7a5dcab27b8c74adeea8b59e7f89cd69abfa14347aca9bba55372db6b00b200a8db7872b9bba48abd54c11f04ef1be49c6bbd58aca9239a77f6066b23" },
                { "ast", "40a4cab3a1c29792a094852de27d1b0aeed78b8ad2e610cf6783163c07b663dd2470feee0e7cfce578a5f8a0ae40c7c99bd3269449933e669d81aeec528cfe7d" },
                { "az", "655eea52b5f9456d4115120694330f6693730dac3f0926b67b2da4179eec275f23ac825555472bf0ab90a0c0883fbd2ef3cd2eec946ebb20ea4fe7cade15cffe" },
                { "be", "7658ec972b3f77b5e3d6245c205b5c4728134bfb28cbb73bd9b89a4052e8169e5972611086c5991db513f9a0880bd2cf30f541ef8accbc182b16ddcefe93c889" },
                { "bg", "e3f112a1ec3e1d04fb6027aade8f118f9263489ccf24a1b0c5256ddc4bb741430317ba2e21662a4bda58440dae5acceea2662727ea84454d06ae8b2b42a5dbf6" },
                { "bn", "f0d86205cf805b2c9342213a6a91335aaa7150eb0e02dcdc2d3e58911bca620a47390d2ce3a9d0357cad48591c71a75db7a97500ed5d0f4823ecc709fdbd56b2" },
                { "br", "cb20639c0bd3b9e350393944ac2b26f355d6f6b5fd2ca141f5f9ba7c1a8820158bbe99fc8ac60b6139ebf26bca82626167309ba783c498888c5f34f7a9ab211a" },
                { "bs", "ac87a6e979ab822149bf374ab79b552139eea01dd039275dc7acb8b69864f784b86ae476b185f8cc45e35500682e716b3528a8b5d91342e5d717ee1cb37f1324" },
                { "ca", "5f616304ffd5f47b1a3724666e715bf54094137d560868b3b09ea8b92e8416de133aa9734c946ea1ae0c90588e9663300c2e92d18b82816968aabce8df80c893" },
                { "cak", "7f59ee24b7b018e34d8cbb362e5acbe62cd859afdece0892f53961ec7277b863cbb26f0d2dcc98a91f785d11f828a1f8c315676a7a570bb56571a878e2fabb9a" },
                { "cs", "36259d8deb2655c258dcd179c156c8adb5239e4e0c84441afc6ed157940373afe59afd5b8339d4d59a6fac3b1a27e6ff1889cfbb38974b960f71e7459b7fb820" },
                { "cy", "2c2850e8852104dc42c1fe331d3af7fb513bd1ec876b4a0f83c729de7c5bb13eb31dbd043343b55f6a9e624a6869caedfceb08759babfddc0af7c3c045f239a9" },
                { "da", "9531361daecac9782b4cb3c9ffe9eed466941f6e4c3fdaeb84efb2e844b51216cd27d7a9ca08594a193d8f6e5939891e6d388d8177a735112451e90113e1e70f" },
                { "de", "82ed539431ea3b2af73d4df4c057c83bc333543f1f49a992d1a3bea36ed6d6966aa24226c34ad4d04dfff7af761e9159c652884d1c7cab4bb47e4528a6b1aeaa" },
                { "dsb", "4fae8365de65babb0cbfb6f1dd97abc4c2619f41c20ffcab0c1787daa59db971bccf1844bd41ddee728dea17cf187d6d8a0ea4e058c992a838719b7a17a0e86e" },
                { "el", "fcb8e3fb3758f32f58deef923098d0f78f2c96b00f12c6830673ccf95cc71365ce6ce7c64e3edbf98de0347e708be8b8ea1b5e9bc7ac5c3565c4fe8d5dba00fb" },
                { "en-CA", "0eab99619d7412d76a8811c703fd2fba0597a33d08ca8851df1a0f2e1ef157600e12712b42240f838e849d346fcd926aed1e00c1c4b2b8a76a431df13ea1f5a3" },
                { "en-GB", "0e2215b77245b23172ef2b239e617dcc28589e76755954f22861ab8aa3deb8c4b1c0e91216fe4ea2eedce0490d5663f7818b6f145c5d926e9aa12a05dc25276f" },
                { "en-US", "2c94d32a22d5f324931013ec5cef7681563750e3a4676fd5bff3d6c59d2e306f499e11bb96472b632af3e588f324d086ff2a321d26e6cdffd595cae5a130f7f7" },
                { "eo", "1594390e0958dfc45bbffbf5b5908185c2c1971647fa8720299c7f22a0ec78d0aefc9d90dee23947f89991488b17152e2c36490e26c3031dcf20c79f774806b4" },
                { "es-AR", "37830c80ea8ff40506d58f1e33ef3fba67c8bb6c3d47b5ccfc8b14861e7e9eb0f8f6aca6a201b9b50c5089008a6a997db1960e2d2cc0529284efe2cdc5da2387" },
                { "es-CL", "8856b414990610368719a6ace2b982bf9c78eb334ddf86de6482bfb53e793db8fe3110fda4bf8d3c30f5862f0c7d6426b9a8e5af6ec8a001269de7f635f10147" },
                { "es-ES", "40d6db4be9b3ccfbb72ab0ff372bd5613364d7625a19f786fe27e90d618984f973066618138fdb3337725f27cce257c1f1990290bd52de7180f2fb87466b6a2c" },
                { "es-MX", "c6efa01649d8f89260e18e3d3df5c92f26c5b67b0a40f9aee8c5839b3d4d02e238f91ec26b7881da5df4a9d8b6243fd714b072274c1296344d78904f26e9af79" },
                { "et", "30057822e6aabf26ccbf5508c9986dd4161e41e274b6a83019511c51f4f2893f6e9d2173a07788c437dd0193495ee84b66ed1b8cd5431c5949af2c720560cb51" },
                { "eu", "03fded5880633a2ba9856408995cf4f98481a3b90aadef7325b1d512b1f351b39543e6546487154e249732828470f87811b0de9a8cd59e09a1e875ec205ced8a" },
                { "fa", "f36e254f16f84c85c4198e70589060f6ba20b88a9085d0388a375932f6b441b27bf195fe7a3c8aa6efb0ac53cf3ff2da48eccc2f4cfd8750b1208aaf36687ee7" },
                { "ff", "370750fbe8725abaf15ae4a9db8fc43d8493b97e5c0b65abeae359d061d0131e4cb1c38b0d02fce65c41ec7ae0006221fbfa2e2f7365630f198f26993599793e" },
                { "fi", "f3f46d2749b2eff58b855fd02c909252f7a1428ad5e05538303923d091de380b28e25bfe6eab6ccd418255275ffa57aa8d030ec8d9bf0c76a1014c712377fc8e" },
                { "fr", "23dd5cdf61d361070d8ef8ca25db25c166cce7d92fb6e306ccac2ac91412c9a37737657b050175b17517123d16d043bdddacd4d6b9def3909ee1538bef9754cb" },
                { "fy-NL", "715e63e56593e883b7957b2e328ce2074a9f4dd5c6598c97c72506135b0409eb312ba841980476f360632c95419de56ae242f8f00bd56a30fb3e578185d94187" },
                { "ga-IE", "a249d4c9aba08d5f83f62a751e21de40b8cc737ad834c4676498eb1eef77fd3bf5d28781a336e6cc93c5956f2f16fad95b6bf8c82e8fa32e1bc9506693cf3c01" },
                { "gd", "1f9e73edf94a1b38a746dbbc44f04588a7e8a1f023fe7e17021302a061b2f5c5c08f6ad76b2d41d9f15bdb8075442426196b4c084aa528228120b6de8f9976cc" },
                { "gl", "be3998dde37277034268f655c062fc9de1d1b9d57736adfdb0703fb00e8f3c7418f80eb2764171ed6a8415c7fd62c7871703d9b6e5203c86d01cc48e80103e00" },
                { "gn", "135b386f839dc690f0a6124314ce183c044145ffb4eaf928a3e5d43ca98c0c2233cdf106de95dabaf0fca7dd32976011efd4b27bf51271450170cf1fb6363bcc" },
                { "gu-IN", "012a254f447c979a3fa42ffe39e6e80efb46226a615cc9e410bc8692954203a1cc05ac07cf8c8d5084dacbccab8721d3ab03cb69927ef46a5c62c96954e76ffd" },
                { "he", "51a7a8fd19479f9468dbeec9c97d93cc32cf4079a793768529f8af0b20b4b767ede1baa35fd4a3b36ae09975b999ddfe250702b5e31a933849b0d96ba5e1abf3" },
                { "hi-IN", "2d3eca6098ab6cb743430e7fc1853dcd9652011a6d812b9ae3af9543b23d83a7107f249532f0867e1d997d8f20766eb6ed607cebc01be3f266025d6fcd46ee5a" },
                { "hr", "31228f7c2b9d479096102524790c6f39afb3235b4510d25ad1975121e2cb84534d1c90aeb3514401138a8a001d55c60f226be768fc347f27dec6d793b27d0c60" },
                { "hsb", "1334618506df5c2f2f9e1be3b6f427fe2c15abc743de834a9517bb064f0efc3edb7b9fe4038e4e545a8b0705129d0f90efac9e60312795b72e6cbb9b56eb0cb6" },
                { "hu", "507666b1974cf6caba4116c4bb425afd482fde78ff9ffb66c3b865f2e06de12f2a9f23b4cdc425f8482a6e3e1888048d29a228d27f555c56be71c2fea1816cdd" },
                { "hy-AM", "a110d6e204b34642c4fc970b3c51916065b9d503bf68173df65a72cd2eeddefef213e3d2642c9304ef0a194f7689aeb11ae57b944bc9c115c106c4de0582a326" },
                { "ia", "c60cffb1b509ce912fdbb00e5f84d898979089f7c752e0f49ced53747328adf6e98d9021defd932b94ab6f93d28218b0c775943389d71940047193a8799eb1a6" },
                { "id", "f824d9166d75576ace78121aa31ff78de7fd6eac07d6b46e58f34cddaf799eb3c25842a6bed73d76452af2f2ddbd80599233ae91301edecc6dfa1efbbbdc1a94" },
                { "is", "baaed3836cb36964c019d4e7d3526f348ed4b1c7c439354b15913c838ffcd9e4e22f319cd4faefda93f57f5cbe7ed8eeeedcf173542774872b4a9e0db4ffa4c9" },
                { "it", "8a19349600253dea0757a978f27827c0fdf71b86f8f4c08bd3d372c716a4ef75e41257627f157335de48fbe2ef32a56685c62b68b0011afa96afc09b1d0df497" },
                { "ja", "b2062ddf0be71f793b14e221224ce6ed531356f94a22f9a720302285f84aa02a0faea6e80d7ec2ec67929473bbaa5f3767dd1aedb0ebae59ce2b99d149047b7a" },
                { "ka", "989bea6280471218df2e21382a787a893cb7675f2981f85c1dc5913b73ffc1186c8a19091b0a86e520caeb6f96da7c9a7ae1ad60bce2bb453e760e5566e398e7" },
                { "kab", "0280e459a3a390a479b4dbea08adb5e0edf1f4bb55514c8a72e623016f332dce6f5cfb9591203d03457be4b5cd4fc040ed6a179cb9e7421c7b57674c9b323b51" },
                { "kk", "a0f7cb062fa3267f7026ac648f2ec771d66c26d209da9163aed7a7b3895a5af935d70010d4d712767038836fe5c226a4055af86cb613b9e05d2ad347945daff7" },
                { "km", "fa4f599075e35253b0a875fc8402a302d8cceb24aa43933eba7855ed03f6f2e9a64764ca7108e4a1730b5833ce10574026e3a6b930d5d2bcf5c0a192564554cd" },
                { "kn", "5734c7943862652ae618aacc51c2c51a90cdb942fc21e9e3dcbb96258abc68b9a5e0ef85ce5fd358111b607866cc06342f8620fdb4924251e1eab37dbe7ce05c" },
                { "ko", "3361062f5f686706b9d33c000da1aa986ce6d4ec4383a0ea6d922094ad8500bc5eed0b918ab7decaac325e401f855838bea2e9531ba58022db9447bf4af6c444" },
                { "lij", "3673fe3121bee31b5dde55536391b650756608637237d470db979eca45adab21847d886590940b22be042720fca35c500e10336f1683dccc13db19db54f5a04d" },
                { "lt", "d6dfca9683d0f091322fb817fc99428e2fa3a699a7523b3ff70e8b4c6a449685bd377b579e1e44365061e04e8b151e9d9baf9761b617f2e4b8d3ce56c3d0fec1" },
                { "lv", "ab5f1f0705046123ff33bb0dd9247f196bf082e26bd1d7724efb15d77488f49668724980280b0849aa1ea400303ed48a85617acbcdc1f04a14d352cd14ce72d4" },
                { "mk", "6dd113b59d166b4cdbb050bb87e77e915ce6305f7ddf9723198f454f13ff2282952bbc91c749a0a3de206f9b51c6298432575556754a843e2677ada86989c50b" },
                { "mr", "924a2d39dee5286822cd5fffdb69e2b7c4e1da4dafa68a3f38936140fcebb2fef330f2ee9ce9f23faa67c8980b2d063db6265354608ac74ba1c0b983f1c3d447" },
                { "ms", "c11b9fbe0235e99ce0cd84383cd46f07fac1a7fde0e97c6229963a8c71179c3b2d9734fb945a9de3935114a3f1a44656b33dee6498cf72e4dfd29b37bb30cdd0" },
                { "my", "61825d06dc0734be46a11e59abacc3a8e0e906dcaa0e7e0a612c7a5522c6641718d9481db7edcb2bfe2b61e885617ead98ea1ca4973160687561752b5a2ab12a" },
                { "nb-NO", "95113a9eb354a731da1fe06efd831cb3fc87487dcd882482c4b942dfc9519cee51c1868979777fad81078c8fc595b6e2e60530eaeae072fbf86f02a9accca63d" },
                { "ne-NP", "6b20fec8ca47d800f3dce79b688af86e825a5de23c97ce807b8f10bc04d212a286b65a4bbc6f77395564d7cfa9465bcdaf5779f87a4e10bf84ecb3037596bfe0" },
                { "nl", "5b82c8625696cd19da5db6fbe8c05d58ce7bebff7ddb5c90c3544cc32c7ace14a2d74f776ed00f94397e1dd3ed3cedf59f9b344b7d38835ef2cefd29911043a7" },
                { "nn-NO", "9a1a211256b426784442c9a30094409991a15c568152cd7449244704ef508dc948b38df5dea03942bbdacc7953d30e72b15cc3944ebbb5b599dc8ca1c2f6ffc0" },
                { "oc", "81d6968a6522f413f77f65eb23edf30093508f64d40ce91e56b00614f61dc57addb53715d79bf350e2ca36d50c2be89446ac2958375f099b66839dc39bb93a19" },
                { "pa-IN", "b733daf8c1410be31b7d8451f7573a1fe453fbc41211fcdb1cf0507a183edfd48431da9ee369ca2d3cee7924d0c9f3fe2e98b1dceab7c5f874b24a3a48a28342" },
                { "pl", "5c296c649ecf9592c3b007407154a1744a1b75ce2bb8323b27295bbe38416f4c037370fe2fde8b5fe797649308b59ec06948a4bec91f30ffc9a5054beb4415b8" },
                { "pt-BR", "eef7ff3378ba865f0f86f3a0a4fd2d0834e7906c2aa8a4ae070a1cd56bfd6b9becaed3420500ac3f1b923363b94e19350a3f40a6cb6f96d45068ae5890b27796" },
                { "pt-PT", "a17f4112b89a54a11acd38da6d019ddda535ac60ecd5f200293a16e6f04e4d25150a300dcf8dfaf1b6c6fa569d0f0addda10403628c36ca36c50f09ba8f3ed0f" },
                { "rm", "74e1c6788d617aaa2a660fa7ce32ff6a7987bc3b042dc2181b0af97895eba1bfeac34a62691a07a60079bec4c894ff3c7d1bfadc44c9e74e81dc9d642ca35541" },
                { "ro", "3370a5954bc481af0894e64e5896f47be3fffec351a34344f168f54642ec1f08a72e31136b9fe36f10df0c3dcc132dd02bf3a2c717a6b454077a8360d6886261" },
                { "ru", "b7d21944b2b9c0bfb4090e6fb891c2fec45a5fcb38eb9cd35992b71269fa4679975d660a402ca0a2f83a187cdf317590d2e1a8d3dc3002ee104bc2077e590513" },
                { "sco", "85308d69dcb685bc11fdc693c2d1c0527e3fd81be54bcd31a6c3750f09e580bda4ad712b1c506a1705bb497bcaf1479cc38ce8536bb7984a80466223eddc4b64" },
                { "si", "1c7c35c70c9a63d39932c2f4abe8f39da49d6899b1dd34ff9e6bf7bec084d4e18e1ebb4bc565895b4945133007586d86c475cc5736d66cc2fdd27f14872e1ac7" },
                { "sk", "601075671221e044f33a1ad0c4f572c5be07d2abad6eff538aab6f967d1615aff1353889f27ea6dd33532f8676176bbd88bb55acb509dba69c296ccff6a5b20e" },
                { "sl", "d9689638e6fd8eceef035b22f46137de1bc6c08ec53fe8e8c7c6b32d1c99040c20ce92de250fed0e2812f1f898b49a5e467c071714ae40268c82e195196f4598" },
                { "son", "3813f2edc135391d00bb60aa4f3d7547837f47b426df5dc9fb0e5a2f0538a07699851793b1a9fc00031fd6ba1cbb9df84cff122236c18c5736cbcb0e55bb3958" },
                { "sq", "5c920bcc9f13f7d2e13eb1229c682b898a65f2149233834b9d784fc645876d88455fc8a16f97154b006e54e3aa7f23f40c67d926c3ab9f33b3793545a9f42570" },
                { "sr", "6c039c28e12e3fbe2c8219ce70a7507d3f17e0c01bdf9856d76acf73b36ab7e7a2bd84986713103b108ce9aa9482d17f084f7fb13c00f79698b127ffd326cf3a" },
                { "sv-SE", "761fb3b112ca048e63b9c7deb7281eddeec6241e718a40f3c578a37c05723ccb37b62697f9d77209f3f0dce090de57319725e3277c2e848acf9fdc3a756a2f00" },
                { "szl", "52c566b4029cdfd361dca59a6247680b9db45a3caf5ec0494687b43a6ed2d742fed2112b31642a8b6d161897d70efcb543f5975e620e798b86522322a11ea9cf" },
                { "ta", "4ce348dfbcc5a63fc7e1e624c5a301020e39cb51ab9102ee1c09367e9982dac410bb33362f3f4b4b45e30a76a65c3c3998e6e16994086404aa33b689ab0e33b9" },
                { "te", "8a93e593e119408d660239f62b56761c6cdad4d36c9aa86556abd84114fa8fbbd635917c753771271d4e06b51eb632bbb7d1b018d40aa4cb957de1041bee6f92" },
                { "th", "e6039560d7d4dc35e117377d5f44a07afff46bc0b45bd45d592b12b502fecac60d4707b81df0b1e4ea3066ec65c74f7375fe9515d1469d21737218a3db69008e" },
                { "tl", "f104526b6d9543f5d7240915f7750e413e857db3aaa3adce2b7d0b7709c123d5f4a074290a8ff7565dc4134c22294a8c78f7716c3b9d440d88b04797cc587f59" },
                { "tr", "299b9e5b76268e64b380e2b5c8d9f8e57019b4bd8c2ddabbf4cb0a4f98c9439df07247ae880fda3aadf4f2435ee95f57106e4706ebed406bfc77047a29220baa" },
                { "trs", "bd82d68c04caeda8ca0039ebe32bec50bd4b5bddc680ffbb97d8ca370e0b9230febcb59b84058284d549df95cb82797accacf274f0207a4476a9029125cee160" },
                { "uk", "66e6f92368f506827f229214dd355607426d8e93de510bd6e030317f46dbb3ea2e9c54c2ac8659e5d537b4546340627f31aba03360a76ae5233aaedba60e472a" },
                { "ur", "d8d57a7e00252ad38863221190bfa79613ccdb8a29262a797803cd72928b0a931f3c5ff06325d1d3668bce46f2872df1cf531fbfc458fc2ffff081b6397b2b0a" },
                { "uz", "98775b67a253d018dc1de7909bcf1f612ada192c88762a7a86b8715000f7da2aa9eed10dbf325aa8da84404b3f866f3ee8a0fff1aea22ed4da3ee70cd3556902" },
                { "vi", "ad57ceab7346f8f5cde9429db98bac570138b9913786b92b32739e23f0f0a3139fd1e8d2d5eeb0f879f5b68061212fb16d10860842dc29d5a484ea204a6ea181" },
                { "xh", "29d096f61fd7e2ae814cbdf569c06b4f74b28a0abad7714a3c658c624495fdfa235d0fe64f19fa707f34ccb37fbf586bd9681ad92ef7c9b5f2d703b9d945781f" },
                { "zh-CN", "9b4dc615a800a8c52d356f62a92aee3fcf3e149a6df629cc40b2272c4a9a3fba9c9495f0e6d654351141cf8300adb38ca05771051223b78dd792c9a38e3a8102" },
                { "zh-TW", "3a8f7f197f8cfc8c82dc601193af640d345f7a27a5f03cef7d6a3cbdb293626f1c0e4d788e2641bad922167579bdcd92e6fe389026681b14de81f6bb00527e98" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b3/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "a86c4f904b0dc68e7b62ab5fba5fec434d131a724af66ea2729bc3c03dc60a2bef0978aa7f0e93ee0b4cfa5271f42d04eb1378c7e2083a35720b7f646206e72d" },
                { "af", "8c1b252f03662a10d642bcbe1d63c31f9da5e340341b38a6989b5854d2a15ad3a2439c8cd50a451235179b9f69edef4bf64dab4d3306b8aca5b7493301282bf7" },
                { "an", "0bff68b28a9ce172d750b8e464b2f04ef0a802efc7da23ea54e1c6c70d4818b16f181e7cff63d3e57b547b5a400182760508798aea170bde0f2c451cb5af6a70" },
                { "ar", "fcf64a3b40c3f9ff5d8ed3704d701add86c2773fed72d65c0ea8c203392f7655ca5819b539337810218e9aa194312764bb0dad7592c2c273c5aa98042d871dd5" },
                { "ast", "89efea964925ab917ce75131fa351653a147cb22d8d33d66e66da142f472ad3318cbbee99cdf94e00daaa8b2563ed681a654ccb7e3943f668a31faa50f58567e" },
                { "az", "e778d715718f2a6951f001079367e64f7ff100de3d6378c386b5e29181ac05995a23799f9f1f789a98eba0042be4df5096768270335ec86d0b2caeef570a7b67" },
                { "be", "af9ec0d3d1a97be33d1d7adb29785ebd508b2130260f0a06fe69f73ba0e965ef9785f3a690e58b61aa71a732b4b47f512398858e5ed93d695b4698192e84a3a4" },
                { "bg", "4eff4d761ecee6f3e803c73316bd37a520a5a4d83c089ad6ddfb8678a603a8c21bcaf2efd79ade3cddd993f530a2412c554adad5372625c767daa6f4cc490851" },
                { "bn", "e75810effcc512f25297cd148e347179cc5452cd07e982c9ea545a749682f8978f001da5b462d5d38cb10a2be9f471f09f1281b7a77d07b8b6aca2b48a8312a6" },
                { "br", "b90b8a687bc85783bff127c80db7766d85e688db57f38b55098b04a44b4e8f41672c4609a8d0a72078bb7ff9eecc68a01aebe8861e617924a2e82837819f4163" },
                { "bs", "87ae0e4d757df78be2cd6854c70e6b89b002f92eec8c4cf8dc663a8408c877e434fcda4f20d36c9726344a64a07dac12cda1d1291ed5493bf104fb1e379fa478" },
                { "ca", "a0a7d277c1bc89698e17c12b53c59ebc0980434a451ad560d09e9170fa4343493205cb96e6962d904c7c4ff9336fb407332fa3dda5c1cef08871eed6207ec230" },
                { "cak", "8e52527088d2e2fb3bc7c420f1426322b91d7b9e2c964903d434c1007437c5c666eff8cca74a6c2152e10725a8831a93e96f686d46205cc28f6172638b18aae5" },
                { "cs", "4321548d3dea2b542cda7776c7be36db4058ff7e8fdc2d1157395ae9e7a29bca36bbfe217bbcbd2175d19dab8845e194866febca6ceaddca993647d8f72b5316" },
                { "cy", "c12acd4ce7a219469c8b52c73e153bbf7e1590a6f4957b22c145d9042acc5c6a265a8b8b28103794a69229f1ce0104d3020e4c1d92ea7d9e40e525f77d9e736b" },
                { "da", "775a0ca803986094bb9aca4bd1f4c0b80e389a63c8fa1e37e17972b5aa45dd5cdebef0a9b5a73d7023a457c461045791bb04d5de1200e8a3f814434e407bb9e0" },
                { "de", "dd0cd9070eb5d16d0db26be9154918a5700f395e74761b445586a1eac81900f8eb2884185286a5c68550a5f873955c899059079ad778abc7566e5c684e088b1f" },
                { "dsb", "fbf88bfa3acfef5dd693dab5c18e08bb7c445aa880e77cf0405809588c0a0ff0a6ad544d45b757e8768aa93fcb559a6a62240caa6b46031f882c2539fdf5d007" },
                { "el", "a06a83188c0a9de13474cd58b503ba34b3871f7b842b9f2345ed008be197dfd38ca2877196b5919fee28ff8b06668e47e0e42e276a3d4a5526e86ad2d460bc2a" },
                { "en-CA", "1ef22349eaa5781fc0e06e919a016279da40b5e8ed3c0869e7acdfdb569c6c2fa7e4d835b7d87b0ddd5cd00511af1c7a1dce26ba0e7c76b68792b56db642fa2d" },
                { "en-GB", "b7e7545a121b8459a69b4a55fd334ec81f0854f2188996160ecc58706e3fbb6cc713ec7365116a00baab29ccd70355e9a60a5d9dd929a2840340d7d6dedd1c20" },
                { "en-US", "9425049c611739fe2408c84d4133ed44428f0eeac4bde66aaabb6273e122abf160ec202e08bb0ede4fc909e5e439b92b978e45bb216f9dd0ddc078e615290710" },
                { "eo", "b26b7655f121230a1881949dd0fafa587f8c805bb925a02ba7daa30271e8f2c0dc3d069449f6964812875cdef03acf11c7dfaf6b5852f90e308800ca68ab85fb" },
                { "es-AR", "f82b99eedfc4bdef2006d5eac0d7042321157b418b92a983820f1fd059fb1c9bbe97c01f0aa4d1cff33c90f0dc7e48e35d2ccd234817a502521f0548c163d922" },
                { "es-CL", "7a9e660e86a5f76e00efe6346b286785af7e8bd7f99393277fe0ed8e62470b68023ab75e68238a3558ef3d285fb0a74993bf8b5043d05ba0fb5b20e6131dbfc8" },
                { "es-ES", "b3a202dc0e2e317ddc02812dafec03d418a40e596d37edcdc355ca5e097197632f49767632725c2c52bc74b0e2f5ab8c7863ff2566cb261b8167618a1edbfd1f" },
                { "es-MX", "ef737b7f6fcba944b06c3a05d037d3d9dde76a7a70c63d06095f3df3045bbb1456af3f122bd74518742d9873a618c6ed8e5c188e79e77c1089ccacd654959d39" },
                { "et", "01de7c1b14e56fc301b25a2de2a5c7015fe5fecee37d0484d7d64c00aa789ee96faa66e206d672f58462600c151e45d78ba03c2f4d3181f04ce2528a767d40e7" },
                { "eu", "4dd2fd1f7fb1396e1c2e7ab21c760b1eb9ada6c46e1731cfef3300c402ea71216abca3f6c5b5ea72bb1649955e28a327dba25606b31e4f20005c12e672219d00" },
                { "fa", "65ab6d593a4bde0c8d4694d45455e2e1212ffc7b15e7e5da78fbc9b14903fb3daa4daeff484cab246dc77a8426e0466f35d33c46a00b9626670a88b648790071" },
                { "ff", "ada5bd91c9f5b5b796061e1016aad93220109b2db79e35e88cdafbd45eace9c41c0cc8621a8324c5c484bfda56b904b027a92c550994c91a56986e7c04f8b267" },
                { "fi", "ccb3c898a2bf411ba730e3cba445ab97280956d75d48808e5dc90f76163ba9c42a2c9386db8159240b8e2b6aaf802b289430d1ae9b18eeadd2fce84d9d43fa1a" },
                { "fr", "dd74aa316f129400e3ad396938eb796d8791e805225bb40e6a96e8ab8aac8e9d1b457aee424f32867feb2d5bd80ee647c2c08d35d555f8ca19eced52d4c1be14" },
                { "fy-NL", "d972daee96a8e01a9f858608f01d3309cf23faf0767cb5a5ffa1cdefdd60450f3a2b3e011dd25d7c5e1e27e737826bf9aa1542f448a4fa253a57f4070d642193" },
                { "ga-IE", "0d8de8f1b6fc4c0e9c71efba45f40914fcedabc02700a30b9df1404fb8c4d8f58ae250a569e96be5bf0f221f321b3b3e56d69ef61a6908e634394181c9f43853" },
                { "gd", "19e6ba0491c6b08064fddf37a36d7b3cc17fd0dcb8ba97ee98aa7d00238d70fb461d31e352a2823337b3702e33f8f5c4eed8a7c07c0c39c2e63d4d8a096189bb" },
                { "gl", "fd2b445c7715d2b08b44bff2262edb78c7f30c4b9132383cde5d1b536dca11ee08e0738a74dfc4d0fd8b219bd56fb62d16041c340bfab1ed135999309b180ef7" },
                { "gn", "468d35e976b6e0232edcd0a8fc4b50ca93a3b78a6332a0eda2cdc4f0a1708d7f864320a7ab8144046f0d7f7dffa7a8828d17a483098d7a36eea7c43b1caee688" },
                { "gu-IN", "4da9c8f557e5bf838e2b880975c131658e4bcd9fa02c58d8cf14dc6b010e51d50616e70827d7754d320786b7ba8885f5d8757430d7125d1aaef97efaa7813c5b" },
                { "he", "d0bfe7d0341488a708ead2f28b918c21c7930cf9bb46c31a8618c435bb2ee107db2040e47637aa592b21a4e5d7b0d08b78d14f452adf18b2d5bd4a540c92b40e" },
                { "hi-IN", "c2611c8164ab948b23437f96dc7ff42fdc751f294196086c29a8d33488608d4ed285fff9ff245741aadc552707deb54d9203d730dbd3a7a942d9daea322623c0" },
                { "hr", "f1bc493f77f2204945e0935e72cda5afc2ff0f547fee3cbe799e8e285c67b3946f0e9e227f1e9e8ddfd613adef04565b8eb5977133e4ea4a9ebe0dc7f8702da7" },
                { "hsb", "7704db9df66ef08a6c2e7965a0781d3feb32720b519fa77b43a31bc288e067af106faa5fe7b151e8c939e02b4796830947ffbbf5f4a73d2fe924c0980221fef7" },
                { "hu", "5f1a940c6cd8dd8b69e065f6e73244fa303d408a5681824c355e66c19fec6608ed80e0dbb4b2b5f14a54b9c0da82d31eab5cea167d7586946608d80e5461655e" },
                { "hy-AM", "6c03522482b9034bbaf05f687ac595bd2eb815e3b3e5159d18f492b7bb0da73e26dc913707a0c129237e05cd518c0fb1bfc59511d326e6a3f76add44dd8ca2ca" },
                { "ia", "c76a649970178f7e6b2e44cc99c399aadb1ab4ebcbef8158d1eefeed627cb7e26aba1095ae5f89bcc28ae09856e0a85f7bd2221c5b8602fec83bda9c73ba7010" },
                { "id", "81231e338ef11e9d1fba1afc0c6663b913a7a35c4899379b93b93c899ca8da41da6827320d1f55fb9c61013b0315486e414aae218f2ae45c1c98c1ebb785e642" },
                { "is", "20c68e7eeaf2efcb3c8a747ed4288a45c223780bb156e8c5291ccd4920ffa859566cab982d55b0fbd85f83a264913f46470279203e956a359ab7c6edd4b18efd" },
                { "it", "ba8de7158e540d2d4127a0f92c1a899a5193afa902da8ba0d891febdd97a6e827f6c35909bf66f0b802f35151f62d61c586ee3552c7bc7b15d5c5aee3b3c6479" },
                { "ja", "d7000a174db0c5dd46035967816661433030c20fcfc66bf8b7af4e065594fd1ec3d2c011ead78214fcff902602b562a98309542021c53779d22a1f091403310c" },
                { "ka", "32832bc3625ed198837308f5e5aa97bf582fddd5aca7b726dba9dba284fce1668776f538180459091701b24072ea0feab4577fa8940af06c0cfc7a0297f9a7e9" },
                { "kab", "20bbf667e8f6d435142c8cfa7bced37008f12e050c56b3b5003be27b1e7577b14f4b0b1cdc1e191f772e6ab1008c37157aa140dfa018018d718c9b179e4606fb" },
                { "kk", "b834d83f8b0d570ad295aad98facd0355dabba534733008ef3e85ffc3e6f1d8980529bfa50e555c6cf4fec6a7f2cb4e9872c98cc9ccca0d7e8cd17021f17b36f" },
                { "km", "f529104f931bda13080c4dfc1e6937fba22d4e27daba4504e162a44997904f3117a6ea4d844ad7b455195178fa370676796d0bc0a938b47613721d17f917b1b9" },
                { "kn", "33a20b1e2dbdb7b5c26e9d05e15d08b85ce0e98cc2c351e7858c94c9e9d5aa26efb99f56d57d99d99d1c9efaf7b6011008360be562d6b32a69eb4ac068b2982e" },
                { "ko", "c373f1e19a7dccd7e7523164cd908da74d63c658afd5ab628babf776e82d50374a8553616019e01c2e53368560c3bbbfd8df73ee5a323b4794021e41659b6e75" },
                { "lij", "3a3006fc5f8b543de8079d7dcc440029ba3f1d11b7908b65eaecf16eeb4e0ec1155729e7543b46ea35340849a6a7affe18ca7e12c2ce01e45dc9fd0a1febe9a7" },
                { "lt", "67ee7bc1e67c17310b61454a3f359636fb7ae521311b912f245bda2669ec08993a565698b72841212ad8d665f61d54f84269ee1931b64658158088c8a301b138" },
                { "lv", "29c4f67f5909ddaaa7967fac786a834e068496a0b4a45854951c755bdef25e83451647ff9269ba96d710ced0c1dca0fc44a74ebd72371a196e625d268e5b635b" },
                { "mk", "1d7953a344e9532052e5316debce02ab1ebd67bc6d606d67073da4a55dcd1936f01b92da61a81335d9970cad7941eef230cb52a87e0305f0fe5168cc6631e799" },
                { "mr", "c099479ced8d715decadbb4c29018491a8d3cca4dbcf088f0be4283fe47c4d691aa97ef4c87f4ec09388bca8972b387b393575b54de5578a7690e916d013795a" },
                { "ms", "1ecf4cd49f77cf1ed7fd0d864308f5370cd38fa729573da5e6c9bb22190ebc6036fd37c7456a3745a6cc25881b5d6449571c2ea5154501b7d2d8d7833e7a3d4c" },
                { "my", "ae7f4c555d3af0318f08ac67ef97fcda75cfb68405be271b13cdca839cae3e460d3d574d8bedf52f6cab6fbce9d86e93058d0bdcb1fdc8f71c39a4b3aa411a52" },
                { "nb-NO", "2e37a21c2fdd476ba68813c8d774e0da3fab1b3ecc4fbc914be7d46027b3f8dae610f08e22205e9e545a578868d7d98705d8ca1ff386270d780a654816fef385" },
                { "ne-NP", "4c33e4a08dd40b7befcff5310c02b69bfd7367faf44278b04cbd57e701f34914a4479d8f2e8839a2556b082ba5beae0d83e1a4b1468c843f4c65bd8b300ef60b" },
                { "nl", "6ef197b2cad6fdf2e423617622c3135c4e80756a76ff1b209318b4e683f93c4a5625c261470669bb6fc9c8250a66cf9f2080d0275787b787c9263c7b19807aec" },
                { "nn-NO", "7dcc35d4c82f3168b9822ebc76b5e87b7dec530a1d9e339b7d09bf6a1c020f758151cd78787b0b0e49702dc8711d291b8db9d31524bb5ba1a2a131f5b6992191" },
                { "oc", "34a66d82aa92bc1f7fe02de258485a837b126560aaee49d2646dab5706520ac005e7293b5957e60f9debe1c28f85ea9f6f3f4ffdec44d1cfa5dad052087e5291" },
                { "pa-IN", "211600b9d984696814e43f49d786fc72ef14ac1b5f7e87a0e0eb59547fca6c961e8a3dc53942d5e7615b8d6cd429c24f55b7942a2c41cc7e767f66aebd69ad78" },
                { "pl", "c2cd2525ad04a372fd352edabb43d203aa092d0634e5b0354db7eed208b7c22c1187772728e5bcde9894d44c9de5513e1341bafa7e109ad5de7811960d3c6f15" },
                { "pt-BR", "f9b902842df35dc9233e324da0b3cbc83c1ff6b7ae8603a57faccf216091cd2041376d7b5372abda5c6e617664c6d0156c596610ff48ac429e08e6b8121b2fd1" },
                { "pt-PT", "c6f2781115b00c8cbc23affb4270ee1580f61418ab22288efb0d8fcb4566967f30fe4658d7191e66ce2e0872336a93053483e0e426de8698fc9f7ba0a5b121e9" },
                { "rm", "90850f5dfaf8ac31f2b97daf28cd3854c09dc136dead403f47f45b15c0daa1e478a30bf616afdd2872926555631949d5635c0eec73058d287d7572eace3b40e6" },
                { "ro", "150256e2ecf8710864f2dfd8d5c8eb9a9f1bc033d0ac761aa6043347106c79f8a737b095b8082460f480f4aea4b3404416ef15e4f3bdee78d6fe9814ec9cf4f4" },
                { "ru", "d8d2175c65b27e23e0d23a004c7a736ffbee62a240e0162a3c25f2f5a547fa077d1d8b29edc32d77edeef61b356fddc5f0b2a69959a5eb1b6d6feafd7ad53645" },
                { "sco", "274e0dd83194e33e2448dbaa09b104cdf3b70392f3a2e1522feae388d62efe5b60e92b4e3e67e737f84e70018e565149094bff06c88c28a4c4cda2ce26656a9e" },
                { "si", "796b8c049852f3485705b4e038a6b4b2e951d84c332c51710ca05df5bf83f66eee8dccea2470b6ec6cb80d68deeb6f3152026bbb25a023c4bb861c389a9e15f7" },
                { "sk", "4b135ea06ff5f7b9d05fa643ecfc723ecc7548eb5b692a9b8c8435d560d16775f569095a4c794b8dd82afff771c45af5f003841b4cceb447dddb27c199c5d84f" },
                { "sl", "8a3ca36a5678266f0adc4eaf3c7fe424236d64a68672c195326dd15246b3e9747dd6ee1c06ac42e0697e63a5d463c61bbe42b7a30dba81b333d8854150b90499" },
                { "son", "30475044a1d6387ee21325fcd07d72045b1c1bcd23a7dc566dcf1dfe5639ebce91ccc8229dae32dfb613a9b3527b24b418eb625350e1ee611d54c29253aa8c65" },
                { "sq", "eafde38534b4ec7d1aaf1d463d5472dd7c83e64465d8206581191cac86b1eaee52dc92d1ca3f05c8ea35b94ff5da0ead7b765aece7122d042b40244eeb8a3c06" },
                { "sr", "f61ffc0ffda58c7bac457db427c4b2bad799c4f6cbc6dfa6d6d2a0c8279c4553c8e7d15b6f6021d62179735c84e77e5420e685e64ff63a6fecfb5002aa6cb532" },
                { "sv-SE", "ac1b0f23cc59d9b865c57d6b626ce14b4b1867e58c3c571e6dce4605ca6eb3aa567a490acba40d0c642d30ce445f68046ed4c6bcdbec87466f23ee43d37a1d6c" },
                { "szl", "1a5ba34dc94e1a8586133ac9875c6928695971fc3974c6194495755105318307062b4e1ac1a6d1a340c18f55ae23d80d08c7f7626405a4b2c3b8b00f9f546b89" },
                { "ta", "ae3b7a95fd103707aa5daf1f7882946a091c2c1a9dbd714a64efaec143179a080ccf3bd8eea1eaac5802f70f2ff601aaae8117b302c4a3f148190fb7167c5453" },
                { "te", "c3b7cf8c5178b6868a09256a03af9b1c02b056505b5c53dbf6229bac92fbd8618266a54bf0cb09e1a19e571090f4e73a6e0ca7bb7b1bea6e46fc07bfa4c1d3d5" },
                { "th", "9ad69d2a1c4613584a026d1637081775f7e07bf1dd164b9068462687c8acb2f4a65ba83b095d7e3a7001bf0521334e23adb815e4cf79bc760dd233f887a730cc" },
                { "tl", "177e7c4e3531fbef209fa9fc57de3753452ce438d7c0c33452865ffc53ced2260b2674cf3e72a0a160ae6a0747d81b46dd5a8408619631f35841b6344c3f31ec" },
                { "tr", "fe89e0c3fddeacee4b26f588c8cf13d22ffea54557daeec78892e443f539a7faf23e8712f46ae24d685ebde16eb8bef01e82d4e19dd017afa33af87d977cc4a9" },
                { "trs", "9445242eaaeb1d152be2436eae2cd8fcbb17f7eb59bd7864d4946a4c2499e5dbc4af0d64ea89bff0ab7526d03d2932758b97516757848a30204e8178fe4caee9" },
                { "uk", "4648bcf87c707dcdcecd98073fc92ad1f8402d89c2defe65e11f3e5b4ef193fc002b00ddbc5a65df56e725ddb454bd92a87181ded911d22265d2508857433478" },
                { "ur", "aa19a1ec35127fb6a5d00184e0d36d2a2447e0ad9cef84416a77207808525550d846e6d1042be7dc6c41f1e6b8c970c9fac7300b78bcc6fbcb5f5fc81e538a9c" },
                { "uz", "4ae529ba8839a55f4041334d1bf1bde465b924681d1bcc5ae9cb6632ddb90e526fa6d97f459f8de2337d5f4a04e2b1bbc7aad97221c8442876722c3f56b9c433" },
                { "vi", "1e15956bb7489dca2db5c930beb7b79e0068561565068b5d71a8379eca24da54a3b9c997ea06c68ea9a3b4c1e8b8b5d4e099fd2cb6f53dbf86d644dd452c3b3a" },
                { "xh", "d663996ce44e02ca250c7cf69abdb446327207004fedb680a8f591f6fbc4747686bdeac63e1277812b9c3da8caba02c78282787cd0f6b302a1373d4817d3f453" },
                { "zh-CN", "c7358df99ab10c250dd4423f36c8e59250570a95b52c29ef336e930bed6587822fb8a995202417a6ee615b8ced89e488db49ba825282988f0dd2da8b5299355f" },
                { "zh-TW", "954182806c6eccd836b2837e0a3e3e98707af5ef4dea6a199b3ed4be81e069f75621f311b452e70121c9c89089deb90628ea428b12533beacd99c429afc592b1" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
