﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string currentVersion = "122.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b32e45e1beb257a07924b422be0c12047b348057f97d263e5365bbb060a02c1d28bf4beb9a7cfacedf52e084cc38c69db5cabf666036d9ac97db90b99ea25029" },
                { "af", "5379ce1f725dfcd02ea185a148281cbd3e892e43dcc5759a0ed22f6cfaab64ea7962d67229a79473ffe71429398ccb9b0185151e6c376b3031a176bca65835d8" },
                { "an", "84a2457433e064632abcd96f8f64c116eca98bbe8aa86479ba6c59e4b79648715a815f62a2f4a47db8c315e52118ed05fb389673d9803c3f074b9119755abccc" },
                { "ar", "f7d2072d2cfc217421ebed4d56f0d833451aa0bd4338d188c4aa15e02261c89649ec425a8a05dbd82438f75da20cb4927d4f4065b6b4eef9affe315be013dc5b" },
                { "ast", "f6211eab56d6d30981be647b7c07b9a131bea2504730203d5f5516be837c96aa7e0d538c7cbd21b30c99fd990315adf4b2ba31fbdf8a6d00e6a01ab4bb3a2c01" },
                { "az", "0595dbf0091ff5a9143e6917cd645464dc11cde3787781da16783c9e3d8e1e7ad395bd7a06144d04b19249efbb877424b2bf4b2641883e0195d383e9bfe760a0" },
                { "be", "27f663bdb2ef184eeacc428fec11f675617d472d95e44d42762889945dbd088dd913a8ffd03e4134aa2b51c850e0fbb0e420539f562bf69baa9c2d491e683266" },
                { "bg", "ffad78742b2e0432889b1835b197b60414c353e32d9ef1c32ccdeb98da7aea81d23805ab487624187f9ce7791c428388a77e02026d17ebb2aa83658d7dd28bc2" },
                { "bn", "68a67fe628e9cb1b27a5756ebc5e044ce8daf43b1e7a784af52924ee4b24d20bb2d1f775cda4c74eb0e53e94b8d56196593bb268e1d31b5e1a5ed1ab1cef1081" },
                { "br", "0e12589750a4822e68a4a06531385897ba74459f9325b433b42d28aa3952dd3395044b830c07c523fd596c0c29ca55972b4cf709901333bd808afdaec3dbb81e" },
                { "bs", "c436e9754ffd98a25274dda56aec0add98c38fd9bafe676858af84639f55c9ba6574d8c43fd3658d0cc70820d2cfdfcf080b8461396763b9d7081c972e700b33" },
                { "ca", "82a6b29f505d4b92d128ce81b340c1b0898cb1c9885aa89e8f4c534ce5e82bf26b34aa5adca0067e66f293179b4e3e654a5999cacd92eb3a861c3730f1c1996d" },
                { "cak", "d7fd7834e2606f6ccd42e15bc6b686297e8170f2cbf756861564565f383b8085f9aca1f93aaa2f6428e5ed11f23f0662d2d2ec58fb7c7e8016eb7b812f31b60f" },
                { "cs", "689e82ca02102c543389aa8e85343f34a5a6906e2758d6a98ade975fe60e94d89011c3f5a8e003d048dfc451c0ec8d665e92fbf953fea45a2b55f27e9f9ef4d5" },
                { "cy", "6c2d5d54a559498570716e6d8e713b1b02db6ffd1dd0d28cdd8f4d56f336f6d8d9ee995766f1d03d8197debce8cf91203c3afc8a98bff94c2b7ccd975b1f6bd1" },
                { "da", "3e410b91f45461868e9c7d198c4142a7a134dae7099a42a77bebd075e80e7b6b31691942ebc1854b15f3dced3dc5e4fd0ee4640d2226f15b9856c9db981f1c4b" },
                { "de", "a00e3ad7cc50b3bc4cb5ad0b3e7644c1cb92edcbf889fbd70bb0b3dd18e2dd00b0ba29588df29b7f1be9a3fb5e1663bf7b8ae49e1cee880ec0d41b049abcf288" },
                { "dsb", "f050a4e5f9b2d119f25d13e51e0b5e76cccad153af54464cdde257ea40cc1daf5e1df82a4712ba29d6ff86b91d7fd2ae9f89c34041f5e6fc53ebc13663b3c320" },
                { "el", "ad5c72aaa260a2a8f040bcfadcd26dd7bc3a68fa875b1d2462de7cfe483597f73c9f355717c82d8318a3da4a05de73a8f68bee2b3c688d5b0b37b3010d8d0c16" },
                { "en-CA", "459df3157190c41d92e5e0cd057b13f2d28f52612ada1378a39f28bfc24b0af5077c2dcc8f985731656dacc88a88947a64c7ae925d6ea2ef7245339286a969a1" },
                { "en-GB", "8341663daac71ce2ffbaa53729a2373e4274dce521c18814532fa982f6fdf638d188a66b0e7a8c0fe1b7841135a0cc3f6908d3f96495e16ced0125b55d23b8a6" },
                { "en-US", "510086e4825b26c6b734b09e6165ad89824820841907b89551aace041cb19d485752d65a2c8e34375a8397818236cdab0f22d10f867093993ff97c1f16f4afd5" },
                { "eo", "3a2ca2b0fb85c1acc8242525c9bd63f924eb1d0a50823f8d09f196ac9a5fe354f1cc6853c73cd43a7965d24ce69a0aaa203d0923c4a43e9c0156690b3af4fba5" },
                { "es-AR", "6b1791cde251077adbf9b7100c41e12b730fd35bf5abbc7f3574467755f3965c596488e7880a7d5d9d59a209b15962a67440d4efa1f2dc501d5fa5fdba08bea1" },
                { "es-CL", "5502878827815a3008e4256ff605d0ad90b1259778208cc89a18276db4762f4903b9a4301856a37a9155b95902786f26337e8861b9bc2c9a5d8e5fef2ead18a7" },
                { "es-ES", "df7db19d5eb52140b8b069c0c1d36b36a8e56313990dc75af437aaeefe9cfc9867a88e2b18c32b936923c2636f00f22b280f30d76cb9c7aea55a190db40f7a20" },
                { "es-MX", "74041b07d4c6206e62beb1f89b9653e4ec67e11f1e1d4c45a8b93bac44ba4a9667e07898a16ba601f01e6ab2049e48f8fd3fe194dcec6ec3c19b2059f3a72706" },
                { "et", "ed55436ff8d5cde081d482640e124f089cf7d7e81a7bcdea30dfe6ddf5d59530b14487a889f6d4e75e159a860c2f21e572206a42a5cfeebac874bf4cc41939c0" },
                { "eu", "d7dbaef54e263ef43ed90a38c96bd4b65d613c4074bfdcfd5cb171ee3578ee99f1695185dec69372bdcd7403ef3356417b1355b66eafa294b400e9a1c0b6dfba" },
                { "fa", "ee1fe706d0435a2206eb0f350ebb528fc842e363c38f3e7d811452f925e9371a87dd6e03c9d8fcf5dd9fefeff8ef9a36df3b6c781d5e46183ef368df0a384314" },
                { "ff", "26b551700e515c6ab2d05d3129939c78db5b98b1cbfe5145f3e66d5d77d82bd7a96a11c39d2bcbec66a7fc61500eda0cebf896c5f2e2086829f67eeb8082b357" },
                { "fi", "871d5d6ce386db8366a19a3bf007259c456c3d1738d4450c740e154056de0204c0dad03d4d26f39aea63d35b7771c5c8c67fea9b00d9c01c5681bc351a45ba98" },
                { "fr", "69908b96f180cb8a0c94c0b1c301bb4699738e14800ca2380d289f72bef9a38e18e99cd9591473888562265ce01eaaca19d4dda10a9761b45b93077e46d97d4d" },
                { "fur", "4fd1e0d72135e958abd19a8708650d4f40286c01b9431263e54207b332f01026b731646fe0cb466a1faa2d3982b1d9b628a14831d8651d4fe6af1974172f3d39" },
                { "fy-NL", "4fbd4092534a51c34167731e1eebd0f44684a7e0fcafea5dbd52c946074896d6d5c6400dd9f5b3c10f75ef1fc49b3969026de7bab3ca461c3e2d3f9deb22ebd3" },
                { "ga-IE", "675f8392b796ee2a2b95497a53b8046bf046fe473d55280d4dc01218e77efae50979448cab795c44540b03423f04eb60083041499432c2625c095adfd64b1a1c" },
                { "gd", "c3be1df8b06e62933692651946bee2153bd1fb3f8b9953a47a51fb7338c22a9b203c33c0f1d20926b26591cb8211f9b786742e448dc06ab4c8217c5d4c9373ea" },
                { "gl", "e2fd7fedccdcd9ff2b144476945c282d9162bbf066291e86bcba387bcc905157e355457360d50a14bba67ba5622f4ef6d47568574edff945a6fe225bfc0fd0c6" },
                { "gn", "510bdcb75a566166b8914ac6ed0b3cb2d6d3c52670cd7d7be06d9eb54e71505e20344552da76007cf9dd4ca9b9e2104e80a2502e4ba814b283896d592f56220c" },
                { "gu-IN", "5415983b5c8b8c4c209193e1bd90cfecc531cc644bd38e391a9e9d2c774b2b5f1a766d19dec5bbc66f0e331ce2b9fb626502a4d45091742c3ad81232be78da77" },
                { "he", "8f6a85c77c5dc097098b6701993df9f2e7cbce7041f7c13cd78902fa68e5636dab05a7f9542ca1b497486ed09548d3079f11a4fb3d00564b6f6536ff11bfb39a" },
                { "hi-IN", "037273e44529bc80d162411e13ad0784e540e8b1f17ae25ab18e7c8c6c3d608e5575c5e88fea34a564e42cc4e89567a5682acbf91b056f97e66b4b9daec4083c" },
                { "hr", "c0fcfe42af5a20ad0207d25064aa4bd6d57223ab52cf42f0c19dc04c2b11a68079383a3b4eb84988c83ba80784bf21cc7733329e0d7e427f3ed60485d79b2211" },
                { "hsb", "5163cf34863db3b2987962164fe7a481775381ee9d5045b0ad0700416ba8967fb41ea8c1badea551bbaaafc015b40654e3a3c192c05f68fc01d1d6d6d75fe5c7" },
                { "hu", "8cb57b7aa018910274906bb60620293d9da9eb4ac9178db3215bbf7380b1a21e63b6348c607b7db81665f7f9f9a05fa40e12ae3272b41fa62ffcbad710d6a515" },
                { "hy-AM", "4c24772f5e684e1545761dc399de06448a10e8e5e4bccf07db44872f73d26920d2c47959f0914a205902bed6e9c6d5bb1601a2eb31f3860dc12a9a05266e2c94" },
                { "ia", "aed6e3d7de34c15a1fcc3428be8db2f23019d3b5d6e4ef059e942fabe3428b926cdecb1e05723990cd1fc0c31db4413ad5d773f5a8b9a51bc70ff2724a07ded8" },
                { "id", "1415ea46537fc1481696f2c603eb8000bdaa92996bbd1a447632ad2ce701743bac114b3879211b30fc30c93f0a95ee85d6066f2e4e1c4a1fe9fba96c51111c27" },
                { "is", "deaedf598b182c0755edc2be246d102e5dc5d24f4963a4797a237acc05aa8aabd401d40cda53c083b560de3ea82d89a75ac16f27cb065d1d549576905277b3fe" },
                { "it", "3073b43a08a49a71bb456b55834eeb9fee29d622b0d8404f23378d931fa026ab2383a7cbb175a32a9f9b3a37f134553870af1bb384c178c028e42618cbc3faba" },
                { "ja", "af28ea0b65743ba9d2281a993fb86613b6391a0debad11221f4c6e28e1359a4a24835ee3be31b00f2d8edb2619209d153a4bbd8ff898f3051bd0be2f158db94f" },
                { "ka", "0ad44c3afc244da9724e880ccba4abba372cadac1ffb5bcdb22a9d9236da6f1e7f6fbff45a25e7acd0c825f981e372b7577704526841982989c398edb9779ab6" },
                { "kab", "9c798af9f27b0c5f10adb47872dfa02f97b43afe54084c79b6742e12d90510c3160cc6c8c572854231b9faa33c30e0a6c295c40a88eb8a0511847b41e2d4b97e" },
                { "kk", "7d805bd95dacefe0178591341340981d37665b3b7d3245880987c3e2ab80b567ba8ceba5e33770e021e1986218971ae87ce24b70580bcc3605a0a8eb0272851d" },
                { "km", "a76d58c38f816a05b432283cadc6b55ec0b3ab3c58b98b7dae5c8220e63ff42c8d78b2e0544d5e1e65a7ceff1b68575638f4381a72b792b3546b999c76a42835" },
                { "kn", "7bd5c450582e8fbded24724f1a06e56bbfd7de78d29fcd1fda3616c3f732f4110038364f66aaca143af5455ef1272bde7dab400f99baf09220ab0d5667c7ea24" },
                { "ko", "d07ec6b047af0aea972f20b9ba20e70e8d967e2cdb9c7d81223b0c63e16c53f4c9e2ccdad9590b7942846ad2fed54ec45c2513a3b5ebbd06c87ccbde7e7be3fa" },
                { "lij", "78ba7b6a0ad3124053fffa4158e667d7bcc58f48d4141123c93baec53dafa6413f420bdda129cc56850631956327e72e092576cb1ac99043c0b5d8a76734a9c5" },
                { "lt", "9b396adf8c5df7911ccbb31cbdba60d6312d56361c9098d6b8c7bd2fdad28995476dbe71ccad3b0ddb9b1b2c06d6c1360647ec8af31e5e5eee846ed75933a315" },
                { "lv", "60864857d5536da8f467a2bcb6c026bbce704c3985670149acae8bd0a1753ca60bb0f8bb5fd2a3c05b2910626672915198f85beba0ca1c62caadac47a9ea4caa" },
                { "mk", "fc97dd9fec8a2c5e365cd3e5271693db8a5630f5dda238b35d06227513de451588ab5f4969123d94dd76f71f813364b985ab0829f17c8d7606107a97f51fe69e" },
                { "mr", "1e1d15c263d78a91b2718b925f1098977a6eb1ff8e1e2cc02d82cdb37faa674d332217f8f5409e55bce9fd92ebb79d1be52214615f37ed34c2a313bb33060c40" },
                { "ms", "a727412647545dc453351cd73019f5303cb3c363d63f741bda0d8aae31661041a06c0969d449801970dc409e6cb5bae675642ebc17c99e0bebdd7c3c45508a35" },
                { "my", "63e9906d588b68924671b6970fdc04680c44c1a51a0fba588564dac182368b3d1a6a68eaa81aaaea57f5416678c3bac49d7bf6af5112d13a3f288bc350336187" },
                { "nb-NO", "1e2afe1577ed56390d55605d1f60cccc601ed43d9a30e325a635129e1a81deb0615394476873eb466f60a9775290a62a9816baa99a1f470bec937dc6c4873abd" },
                { "ne-NP", "2243633af713afb9b62bb8cd1708be50048325e6046fcf5a433161206b2a358af08084a6b0c7a4b763bde16276973b5e0081c1f17385fb79cba2bb68e5a353b5" },
                { "nl", "7aa32054872effa833f44f3194b9bfc2311605b914803c1a2c3c79939655f2bb3f3b7fb61a29396fca8119234890a3b1847c0191c8dc19899f928ac0597a5309" },
                { "nn-NO", "fcf8c7163c0c9807fe658008be98f4a6841f5a1c0b0bca3d557989641b0381122695d3d2264de954a7ab99745df76127c097915829127f726d984541dc553392" },
                { "oc", "07d12c82bbad6105dbcaea44f280f9a1f225b48a0eaf39598abf5cd5b428f1b515033217fcba22d9c240001b27f521022bf1c23f17c5f3fa0db2198c2b66ea26" },
                { "pa-IN", "797b80b68bf10f4016295f62cea57c8b0501a1deb0da9ad08864f159749a97a9602ebe374ae43fa5526d5323d4d0f5cbf8a890d15ea6d3b1178ea807347aa243" },
                { "pl", "db7b69434582344393e1c3db69f3d04410345bc90d96c12d4f0912b899ca4179e0e345f06e00adcf2f2f259858cc8ebd1e9362ffb636701e541b30fb299824f7" },
                { "pt-BR", "5aca755e0fbf4667905a7000dc292974e77f55b9cab6b7bc267b49dd1460139bd0da68ac6c0984ccca69c414f5417870108132c3031ee28c478c4faddd67643d" },
                { "pt-PT", "f11cd27b40606f4dfb0977b3e4e31c89c74d200563008da1551052cd648517b066fe121718bcb64bc605b9afa68c9e120b481b2347d53a0c43ce25041042905e" },
                { "rm", "aa16ce1bdeeef57265787a11af6ffd431a0ee41e8737ac9a01f71843b255d688c9b9a04766f34dd8d45873c9114c63aab28489c39b225734713cf5419c9b7915" },
                { "ro", "56fb3c9ebac8c35c5c0e8187ec4f1d89f1c155274790ac18ac9334186000983c0288745ddbbea5b9a8dd285b4c0a0126d64fb1d3609335ef91d3c19287d148ce" },
                { "ru", "3fb839850f34d6845e6de243b18f1660181a7106977144e0cf1adb78715fd9e7d6d5e4420d54373b38c559738fc1fbc1738239af7fd2eff851d9ef5141162e7e" },
                { "sat", "11a1f56ebd8ba8d81da561a59aca7212f8c6913fa66c972354ce21657d47545e6eb8392270d0d7b4baa5361b0971c865f52e10f7523947500b8814b8bb8f84f5" },
                { "sc", "50372d6415b0000d618d3d7712e3b2b58973ffc4d7b9c4850d0b0c0dfa413c50bb5a1b004dc1b891ada3ef0aa03cae0ceacc9067cad78d4d9b0d9f13ecdd4655" },
                { "sco", "5fcb1fbfcc92b0ba11aa324842f03de4fb6921305bbf0d8db7faa7806fab4b3d1fb2c3b4c9996e540d39111e61af8706a5d6a0cf91703358673a6b18cb8b3a83" },
                { "si", "09ff09e58e20a9fcf79e44af7d5dd010585d5fa503f4b77560fec55f4b51d61fbf4d0b687a8f7a919f4d6bd7258630f02e7fda88afca0c0d6243ed113db27e50" },
                { "sk", "2341ffae017ab99c4c596c0c25c9cab71ee80ee12020c5778bdda69454ad471b4d1ac700ebe3150fd7bc6effe9da0c46e9f0f30d622fb92998e851a455c88f7f" },
                { "sl", "db3249bb5514eaf03a963ec4dc2f47fcf09e54dd49a250e347b8b841c10da752984d724a8a58e66d94d00567dc59751da398901d3f39d28df4a019ef29eb1221" },
                { "son", "aec55405615172a18789b12ce69d905448ece3ba5a53cb2755fd1fff71fb6576f854ec60e7f40a4e258fba87e70a17b6e99dd4631df11b801059d26b756a03e4" },
                { "sq", "30db07a4e9c72daf53e047f47f3bd34a5573db66ac9355a463dcdf7278d49decbaf2f546fcc43d5f0afa325badbe59d2c5b3faf6a9ca830b508185405943539d" },
                { "sr", "ca695739422b971dc14e96288d11cd03954cf4ac57191f9105bcee55eb82d0bc9a79306123083f9492ec86e3626e7664794562fb800b2d834a21cd15891d469e" },
                { "sv-SE", "61bd8bba421b4e507b72bc0c8a1f45819a3a3fea41f3977a18f95b9322740e77a6e456ceeb908ed26ad75dbdc92428c46bd9a50a60d2cd93129c8ee4ce4c8f3c" },
                { "szl", "87389ec4f6cf1f41f99ae83283b3c3cd3d740ab202b8398aebafe071254a0db5d948bc7ffae6bdb699921767d5c7bb4ceb2c4c7363c3c952aa67e2791763472f" },
                { "ta", "2be0045f3f748f3144a949b27875cb09bde91556cfff786e17ec961e0a2df502b06f09cab364f8a97526c1697738709427bada4d9a9de7c24f6831e880783202" },
                { "te", "d363fffaa2a421b46ab3610812bbea2b47f8fded3737cfc73e8be473cd553606dd4ae33412e14475cb77b88a2e0553a74456e246141cb9d0f0d433447e87235d" },
                { "tg", "571560d72f61dd69a6edb21b1b75463acae6b15d967e335c72d141b6cea6e83d66add0650c9ea1f2d688cd68dec36f561cab7b7df79beeb4db2d81161125d492" },
                { "th", "430b6ddc501217f51835b798cb2e240ea304e56e6c243cc87ebe5a07578dabe053b6d71b73b6aa44461d40110db67363b5c961a46a6b668a0a9c0851d88ba795" },
                { "tl", "7f0b779b026946a4c3b409d38d6df8d36582b694153a2906d8e9af8c90dbba76468ae57ab423fc235969ad8c5bb4b1ef4e7f4bad276c1f918cacfcdcaca3a5bb" },
                { "tr", "e7a528decc409fa5943ab582344515ef8247cc44220b3a2fa32a93445421f4f3d6d84e2f609320668dc25c8f952e7fbcab374d3487146be2e9510bcbda524ca0" },
                { "trs", "54e412862e230de5289c9c66b1013396e99688f8aaa6a948b5c43ef94ce4abd73565a76dc7e144b7b3235e61e94d41be2a477488d017cd6646e4f1fb948e5d88" },
                { "uk", "8a15c08df732f3f3fa52109693223352e6ae4ec306955fa59e5c869abca5a2f82c7714419db25ca1fa9e660ef9a8c53bf7fa6c0908ec2e55790f05fcea0cffba" },
                { "ur", "bb0d2707f32e2a9a99124b85fdc1a40e4ce9ab64e8f82600ce0f1d28d161ca4ffe2786f9b33dd7704ea149a612029f34cbbc72a98e580b5cfef99d1ba67fb786" },
                { "uz", "83586665bd50ceaae0d6d1b9037c28d498a1a4158ea841628fcc10c48684173c42a1536505a170f2719ff8140a2ddb9a5273d79078dfdaab699ba5998b4903d3" },
                { "vi", "00c90fc85b9f266b9d40f4c76f2f25ce0e66e1e1d09598c4112ac948fe21323dfb552cf714db9d2b4e200855b1d31da918f25cf2d81d6e9783c3f0d257938d1c" },
                { "xh", "8f72cc3ef3553a739e27d13a8ab32e75499c84af453f6867eea63f8ceb5d5c8cd2422514593778f6567a0c6bde0a87dd9e677c51de358dbfd3755d64d921dfa2" },
                { "zh-CN", "548522dbc4fd9532842c1bf15bcbf7dd798149a0cbdef0d530b16a4811d671c29da66b8dcb357511ecefbea3f6deb258098a07f914b9241733592f1cc5dc1f87" },
                { "zh-TW", "98868b249fdc5a56685534f10c0c709457655809d2ad82caba6579ef5bb29ae7c1754776440cf94505719053e9a45129d394e79aa46f0241397f29504868fb1e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "19ca6ccfbc7cfa6c0af8f5c2195744e606a72a6457b5b887b7be72076529026abf3f8a66135c1ec44aa2d1cd7829726d519d3adfe53ca764cb6ddaaf2cf476eb" },
                { "af", "4bdc878e8df4a02d8101e71744f62b359af09deb1da61afa733cda7a5f3f89a4c7298e5ce3d6bf66450eeca4edbc99bfe5051460ba73281e05d144f0e4b7ec62" },
                { "an", "c521937fa9c3e78489224a79cb62c5343fa9ddc2bf51ba9c24104712a861e2696ce64ab80d6f5a434bc87502d354a2d921ecc55b2df6d817b4f66c6fc082a493" },
                { "ar", "d8fd3e2f6796183213c192ef8acbf948f2507ad289dcb18c3d0c9b08fc4566a89006f62bbff9e890da4f3a1b790fcf6afdf8a163f4dbc484db4a3746ec6303a5" },
                { "ast", "5344244f2384124a7bb447c92ec6e4faffaf998735e5c8d0b4a6d310d49e91f1d42a1d9f0adfe08a0c70fd8ea8db5fb256304650f6a17848deed697954c76b1c" },
                { "az", "25a9d0e4aeafdd898693ef4b30a0d58e0763e44d5227360a13f6c5ec80d39f9aba671607c10cbfd8074befab0fd993a2ac5ca572612d475e6fc6457024f099d4" },
                { "be", "565ff217a4e7d8fdf03f8560a2b5703aaa916f34076e78ab75e1242d61962a2a120964e8c88b4a4127104cba71f4d083460deb1ab97ac48cca3df2d7df997f08" },
                { "bg", "5d629778d2b9492b249c6eadfef6a88b912aab493b3bdfafe91de79248814e317632c245836cd6c609fa0febb7a257216c328f90b792f6190b21299d4bb39898" },
                { "bn", "bdf40752a0d2ec11d3b5b74917f054e14cc1fa8ace639fc40a2104b0bc8639216d8ea0636cdd7a67ab4f3c9a42a74e71f7aef02d4b75b0ebf0730dd8757c58d0" },
                { "br", "0d117eb495f78e1caa06220f4c566f158647fe6399f0029e132447f73663bbe9269a9aaea3446f4e378fb5ea9922767e6465068134780432b85c74786ca8e8f5" },
                { "bs", "0522bef57b2330cbb2c3f2699abe29e5c2bd327351690a6a1170dc45b879225ac72a727edf5733612bace8e651e8f0fb35defbbf98020a7c509ebb1f9e347722" },
                { "ca", "3e46054cbc7dd8b15d6d067ce94fdd565c4acd819ca2538f7f82e7f50a71dd951921ce9da7ba36bfae29612901f10d7e6de6c4bdff5628b3803feb3aa3145929" },
                { "cak", "361c3f33bec4bc73ec16f043a60a68d14d30766ba9bcb0604ad2f4041106d6ed3947e7f26a76776259b3d1c370fb07057217e3843b57a8813f38f4db75ce2a09" },
                { "cs", "effabe630e3af396b7cc6ae3b475e5dc87e13f42e583b497a99c49bdd5a722e1dca3ee1e93ecb3fe270fffec9c0bae267ed9e85df86266f8e3e64a355ef6759c" },
                { "cy", "17a832076355ef503d820151a4bfc413722d83c721355b4833074b56a7b9268554dc11876da41a52e131b4f1d2a5d091f5dd5ea18a87e298b26ae4966112bdd3" },
                { "da", "80ab4ef706636de0e665b7976f17c440acbae1ce23f79b6ef5ba1d6bc2a0030ed4154f682b62178376a91262e7c35c28d9663e5af0c9f65759dd9b4ce779c58d" },
                { "de", "d8708e3d1a39ea30f55925f6f61f286bd8ebda9aa9db8a385444c316168cd506af2c44cb258e9b2e656553a0d2e6c6779cba41b0c6c70a1f20febe746e718003" },
                { "dsb", "ac66b3942ebac84c0d16f2f16f13051d1c9775758454ab8023827e4e9868bd1d1b6b9929422109da2f498bfa847e9bfa92cf3a646e90b3c9270b0248b79c6423" },
                { "el", "826e6e617692b07a729f9d5b2f5999b30288f2fc939e5d112fae2cf6dede2780e04d6f472b15937719a268a96c16dccf88fd9c6374355244dec4f1bd0580e667" },
                { "en-CA", "69c6d749778a7d3681b5f9542495c5db2fd52c3ba26cbd2e7d902db5c427f69e6db4a6581c8fce7a4300a7ef98537eaf0e5f2b09d9486913fa958b5f9603ebc5" },
                { "en-GB", "341fdae804428ed2d218a584aa0c7c26c5ebb544bbde10da1f93e8203d6d78314b232a19f643f87c1e1ff2f280b45694b1774458c251e271a9bb0f04a29eb217" },
                { "en-US", "816311811b9cf1f6ffc03721f8642843adbf0135a1c687a04ffdfff6c5730eec2fc3a9141a9a79e293f425b68d68c99881c4b06ee340ac97ac7d8aef5fe2360b" },
                { "eo", "33e0fb8612867ead55beef02d32eb94979ad16c650064226f20df8dc0f6457d549a13b98caa89a421921e8bc1cc72e7f9e6d2383a8563bccf6be79c100bb1670" },
                { "es-AR", "5b3469b9cd7597baba0028b86ec05bec1cfdac72f5154b74a8bf6614c97a5a5a978f19b2e3eeee7588bd9cb7a4e3450b96611a083a6a7aa6290a20d811623756" },
                { "es-CL", "2ae108df9cb1fd33529dfad09176124e6a12fbb0f9c59b3b401890c0aeab4af9244de0662b9380da48125ab054a930faf0cd99dd1127793724e5edc373f9f763" },
                { "es-ES", "3dd39e3d7e3239e770e41f2597dfbfcbe5b5212f44a92ae9824245e5208acc33eb685ed03c827e2d31a3e5cf041657ad72b725219b01a506ab258079ac26be23" },
                { "es-MX", "824319c34b48e38c84dfb288bd12493faa6de4bb40b278757e400a978f9a88c6d5caa6679222f19d1fd7dc0d080f931436198703c1793ff4b156ffe6765b46ff" },
                { "et", "67b2e8a2dd5a69d4eb56a230eed583edfd06845067a9e77a84bd364d91043659ce6f94f2e4b2db05ba6ceceee59c92c573f4a6f36264ce8e5e994993b3d6ec68" },
                { "eu", "f6ba5c0faa88aed28ecbcdbccffa709cf83d5c40c15ddd30acb363c92019cb2f28edcc3723b2574793ab1c895adfdfae19dda9782f518693cf1205f6f445d9ff" },
                { "fa", "1723739978923b61c3c879b0846440430f6146f5b36d35fef72c80c5686a58abf36f645ea5ae529a3356a463489e2a90049339a410ae59165f80710e3b6ac952" },
                { "ff", "0df562daec022636e251b8af458ccf03ce83095a8bcd0d8c12e17daa3a4eff5e6ab8ac94710389c8d1a9ba75e4484873c89c928f1e3296acb4d4cb08f407a1cf" },
                { "fi", "21a1ad2114531499ae8154314764068a4a38909ac235e195ce66d1bb75c4df5df0a7eafc7afb3c52c8f2669c746d523530a7011fab3f94d56307fdbe6b9b5994" },
                { "fr", "4d89254b0a377114ae8c430d1c02ae0075a5bead88380347d74acb0f99955d09e80efc46e8f14cc322f71226ed53abf75d664b5dae3053c4cb24bc97f7a998a2" },
                { "fur", "746cf702c29dd70a16e6339924001f189be0ca9d866bb929a3749b1d9707cec9f68b2b41c8a693de44d76e746ca049ed187ddc0d43ae4638c3f767ab02dda65e" },
                { "fy-NL", "7d37d50bff1d991f0b5b4f9aeddbe62426685744e263444d72064dc3f20d988048b0c28a152f02f6e2d80aee78755239ca44b84a322042441c66e82dd7d44b1f" },
                { "ga-IE", "01b38f5fda8571664af5eb3aa71240c01f7cc82fa96f61dd4b08be209066b1ade222055fce44ea8ecb0321d0c96896224838397102b053918e691e4c94a7c40a" },
                { "gd", "2fda61e8f18cb65790d85cab6900611af463c056fc7da02b05ed43c8aa580804e03e3d4200adb429b54b25a4f083ec1be3fb531fa7240fe0659746f452a960bc" },
                { "gl", "ef718138e53376de67cb53916fe6d8cad1404f87baf1cc0ece66d9daf6952585fc9bf81bbb6a9151f96f2c69ea9ea2cfec9e3c902df443ec2d50da39b3635595" },
                { "gn", "d0f6c9807bf431885bc8a112d9e69242f3ed2ce495dd91c23de3badb00fc742ebcb505b8fde79aa1cc440374d634813911cb6391a7efd9de25202fef93f9f98a" },
                { "gu-IN", "5315c19ccb4588176d399df298cbce68e2d348eca19ed15b930e4dd61ff4949c4aba0e917b7a4102f191e64e6335f32594b06dea9494ef13d2f8fc1d14702082" },
                { "he", "334fb2c01785ffc4d464ad258f180481aaced940400d58fba4441e42cf34d8503e578e22dc93826a867091b3f87571cbcac1ad9ecfc8870e1a374836d6698f95" },
                { "hi-IN", "b0bc3edca6068aed2e7a594d98963a5885a9f072db59b27fc3af68e5b1a7aa57f6d2f7ab7b638667ac57891e2e711eb47bf2ab024ef3f5ece594bd86f89bd724" },
                { "hr", "2ff9c14a0c1fc3324c73cad96983c306bc459aa1804d52dad963ddaafe4b9a492bada465125cff7c91f0ab3e96b5a4393ebbf67c0729fd50199b549e6c8380b5" },
                { "hsb", "91a4364679636b730cf0652cb78fc92403747b806482ef2352bd90f149e5c505a81fbe900571493976eda28ee35ab6a07203011a2c4a05f9f84f9d6f943b0a54" },
                { "hu", "fc45e96c2f3705a022759bad99fe290bb5d8b477b4c2f885295e1435ed2b09f421b543e54cd0ca51edd49dcd96d66d8054a03f049e38e97c9663e5d71a592b85" },
                { "hy-AM", "3b648e10279ca53c3412aa82d86347e03d07add6d3f70106cb5fab05320fee780fd2fd7490e92bfe8a99bc6cbc4d9e76e2839db68ca4bfc70cca6ff3079d8c7a" },
                { "ia", "c37189080d6847593f07f2d9f9e84b5bc048e44443cea9e8c423f072cf17ad160a8207347217ed1876530b8352bd216b52e80f04879efee6d9f68a76e1f8b3a1" },
                { "id", "eb35d85f3617023a0a1270e46ab35421da3e30afa0b2e4cbf7cbc4bc5a2865bf61b54fa1d84bdae54059d54d55fea8c7afbfc809238b307add86ab4507923c43" },
                { "is", "baa69b24cfbfbb6f3b88ec7916fc9b061e2b99363a1b5fef72ad707450c43a2772ea20939dc78d47b89ffbb8e36f6cdf8c84483b9c874f0d23f7f8384b271be4" },
                { "it", "c61bbd030749b22c4588428ec7e98a2f78ff77cbb57247350bf42a26b3510fd033e06c768cc5764f98ef67e0c89bc38254f0db7cd3b3f3460a4fd461697eba82" },
                { "ja", "18c6eaff6a8bd117c4028b3f324ca6192ecf2881742d44306fe6118b7ba14369b9f9dfa08d4d873b60b6f9078c3a82a02b61d42e8e0299f33a4a254f2e03f2fb" },
                { "ka", "aeb5015cde63aabb1a00217615baf94a9396287e44122e7c13b09406fec46fb7f89cceeec913f1bdfe214f9b19397e0a353a4291c5609b005e0e10e8ca5bd94f" },
                { "kab", "734bceef142342be01cad0d11bcec82d4de87f174d50120bd2af9303ab57a0e6a6c3495ef1575ab028cb95cc84fd7a64a6582acae10ac6ccfe17e6cdf9de0b63" },
                { "kk", "35b99a9acd3755313f39f2349289ec3ccb65404a703a3622ba1c79850aa1d5eae8a447bf6d2641bfe62c1c270ded411b223c834b091a201b8b50c96f1596aa55" },
                { "km", "358a5944ec95f55718ae7e4fc69c516d14ac02435348b5848a9d40fdb7664d731843f83cd88447ab20fdff146978f4f199e0b791793a3744fd9dd6a39f6f28d4" },
                { "kn", "a71de1eb5e125546ae5d1a2cb1a0ebd7b5919990eb0baf7c603e3f184ff43959ec307285f759570f1ab2278c9744e49430ca352b490c83ed50198fe30afe3592" },
                { "ko", "8905e2dee29c69c8a4847afe54d5c3dc88ac80b7201933d0166b2ee91b7e6bfd83fae574797881e679996c6499afbd56265b6170095dd9fbe3a60cfa5d4ca67c" },
                { "lij", "2096ff5d3be33716f72f1cb1d652c77311b165bf65c84a25adf8be93490cbfcff60daf1a36a20b10d8e91d65048ca9a34500047d2f1f4b30f1f077d1edf43c1e" },
                { "lt", "3b65efba19bf20d9654c735f2b15492d88b59b0c67b2e7dc089efa455a820a14b69db39adab4ea2216cb55dcba59370b63fcfeb2553deac3c933473e6c4b7a62" },
                { "lv", "f344f6609c190ba07bc11ca556db5c2da766056133a664cae8ecde768003d28d66ebb86dc7f0b34b105efe50ff43bcb6dbc70d49773e03fa7e521832a13f4daa" },
                { "mk", "706783fcd292ca227b270f3259d264a2366fa0e3f86c0aafc19f88c5dc33a6f537dce420e4113cbdb31fd9d79ca4cdf0d6183bc237c28fc152e517d6c2416624" },
                { "mr", "f98dbd0ca688d87092fc7e2fde973639fdba762a23a5c1e545f378a85b7c596516414815f047d9fbbedeebbf054a15ca5b182606331067e454e2951ab33e7962" },
                { "ms", "c37415fe5c465b4e29e8efc3d225b841fa791b860f11b2281f3a33d958e66ec5e2a782a597b2ae15f75187f6df4a2ea2eb588114ec5e905840bb52901f235f33" },
                { "my", "cb53724ed2fa2c5eec417b4de8b5b4648917157419ad81d4748ba2aa179fadca187a908d8b9e9aabc24b3922ab427bb22b907bd40797b486ada72e975de26997" },
                { "nb-NO", "83278b0278ad9e7ad840cafc4b4b3ef8c22292646249cbc28cf064b966386af6070b3aee0ed3fe72188edc4aae257feef13b00d5c4a03df700cbc6061c0c7a6d" },
                { "ne-NP", "bb2d7136bf40578d0c6553e1941e59446fd0decd9ff1e023463fc376abb080d2e3bb97ac123dd675bdbceec74c1f42ca7177c8bed17061c8ab6cc6ec480e1afd" },
                { "nl", "049ab22e227ed40d1d8af629d44957e64af964341a65145b17ccad3ca693ef27a7bafb556b1ddc7f7ba6dfc27661793e62ba523b79bbc586f162c53aa9158f17" },
                { "nn-NO", "14179c7e75053c2be0923dd6ae64a37a9ab8fb52e05665d76feeca41b58bbde8b884f630179e044e929afea4280586d82dbda51f96fedc56bafb92474e4f398d" },
                { "oc", "a3d13382309d0e6d9f67d0909666b6cd89abd5f05173b169a6f15fde4b89555fcd7177a59e7660a90070d359f8f56b74c1a3c5c3ad0efd05fc4e1394b8ca6cdf" },
                { "pa-IN", "d0adffc6772b6de2a1c856bdb129f2b054a77db5ee84f8f58e2c1edfc07b6933e779107cb6a229a7c8b0a7b2d9608a2536bc33859bfbdfd773d0445f3c8bc8b6" },
                { "pl", "4fd82cfdd65456d13d6439d314fcf5b042dc2e061af3d0c867408b74b58627bfc273d716a0eb350ccb1b0b53c60127479cc74797cbd2326b0e8fb6eb2ed1ec49" },
                { "pt-BR", "a542d0f38649031512b68cf0a99145570163d4bcdf46869511098c313be8941636bfd7bef8fc724c1e0c93cf3c29472ca951beee2062058de45b343350d03f1c" },
                { "pt-PT", "9f2f92395adeb627916cf0b1a5468c5da9c990fec8fa342797a169e974b3c737c0abc9ff042e38f594350918ece5fe26c0c3116da9ab23e90c320688d5c29de2" },
                { "rm", "194d125df0e0e643eb54561dd64c0750813bbc508645ee8fc95e8c35973f371d4482482f3cc465a04aadffcd44e0db8dba7cb5ab1f1a48ace508a5231ec8dce9" },
                { "ro", "0c796bf14d2c534b8dd28b49055e5495ba95d7f2351028e815d195ca938415b1fcb4a71c852ce1c07cafb2071d36953dc0c395b251c361d5151619f0a8bbe1e1" },
                { "ru", "d250e4f966ef04bd58059ad3e6da840946fe2a9652d5a87a10e1f54435a4df7f2d273beab4ab7f1237dc8929f0f41e84ed46ebee7e66e2a4d19a977bf9d1e6eb" },
                { "sat", "f11929365dfe32be154a50ed582d3954a87cd88f822a83613d3f19b52a588f2966539f526defa2ad9441743eea379e933c150d4ea0045e3639637157a2be46b4" },
                { "sc", "88bb16ccf2c94439f6369c7bf10adeb5ff249be301866c425ce1499ae14c4e1e06c24b65c9dd07318d9da496256972bac9c51bf4a176a19aa66ce6b79476f195" },
                { "sco", "db162a028411ab8d07c8a2db93aabb14e3693b788f92a455908440b9e065b1dcc7df867e903d39d15047dbcf0f44515eb7a8d1afa0d3b2847c9b63d790dd82da" },
                { "si", "5db4972f67b32e9349c9fe5df75b2714f3094d00ba130aeb354b5d35e8c5ffce7797a36c88a17ccdecf769dd8532fe8d9ee51d113c790ecf18e98c08a78a948c" },
                { "sk", "5d83b3b19ae4c83e438e67f24fc0c5c9eb7e39d8eabc72e531ca38fe5330c797ccc0462f58a26f6040eb1ab50a5cd8dd49e124d38c176d71ec2231d192289762" },
                { "sl", "85dc4c6a10cda6f3b8aef78a8dd7f2049d7d79f328eb9be17af46064f1acf9ef11b1bf24a7d70b907237012c13d26a69d5ff28112bff408c1fd46cccd448f4ef" },
                { "son", "e35688cceface9448f43f11259625eb8ee71b88b14524d873612918ec5079bf46e6b55880bd232c66fda671a3268cf6513a26e730be16339c32f4ac8cbf287e6" },
                { "sq", "969208247e3364277596917188f437fc5748e12ecaf9e713bf0a69377784159d572e8912a68c6733c9ac2d6bea9c7e05077cd453482252a9e714e528d1a261ef" },
                { "sr", "284cf8af4b145adb660db80b96d64d6c94e11beeecf3000b345cd2a674268e00a56cbe590d9087dad1c0c4e6fb9a9e8cbf6934cffd5cd87e1fc67914c6a20430" },
                { "sv-SE", "8778f4bbfd7aee1cd6526ca10ec7c2f52683ff79c8ab9f5aee73940aee56296085611ea781c9c25b62f4ee2a32d527a779eac1a5229e8eaf887986422d51cf46" },
                { "szl", "ef0cf555f6fd03474115bbddf89551d8a2dee345091431fce3be2f8e31ec38d31ed83926fa23a9478027efcd62438a21acb2d9df9122129bdaaba0358be41377" },
                { "ta", "b031213c86301c64ab118eaa2969b301e7733897e96919b40b11070d214b18c22c1a7d85eb09b07de5141cccf77b13a12b926f0b9974dac37991262c7c2a8cd7" },
                { "te", "2f94960ee20fd2d7722c667d68d46df2d3f5588d803b82761b35000a013de2907248c46901f38578c4e03585abd671e51ac9cb2312ae292ca2042453f708fe4b" },
                { "tg", "a057b4eda34886391b27028dabf1bdc37ecfee00c8f943dda76f01d7ed21342d1399465ad9ab75108fb20c70d080c847dafe710314d41822d51e6dbe24989b20" },
                { "th", "3cee061528a76cc3776e69c94f663ac5cb15bd5a9f47c40136a07c47db56b5fd2ea9aa18201e56548724e886e6f328eeec4b3dc05263e5fad59dae215bcd83a9" },
                { "tl", "4ba916534f7a9c504371ae51f1ed946dee22b40e18bc80edb539c33bbc8f3ed7254df95193aa4690da1b6e4b9ca08e047cbcff47a45e142d76cc712805d15e14" },
                { "tr", "436bd06373381d59de78caddf1013fb0cba5acab3d8ef0f1997c72ff524fbac0aadeabc5f5a80ee448da379906c288e37b97340841beed96da9ce526ba85e217" },
                { "trs", "867afce28a271fa537f494eb84ec850d74332a85c602f94555600e7e5f9dc362bfc287bfd929ed82e8d26c8837d4ba7b8b6f6a2bcf4f14d6f3dea94dce234098" },
                { "uk", "f0dc9ed17ded313e667116c3f6699d6aae26ae98b4fd2e92117823879eb53821ac105cf0bacb0cec569882de8d1ece28787c6289b47cd541ed9f3e3ae1fb5ef1" },
                { "ur", "bb1ad7f9815b32ec4eb76e632c019b52a81e9d15b9c706403758bfc891681c109bfc99dd4b4abd6fac2bf13752ae9ab9e2d084e65d78943b687840aaef3d7099" },
                { "uz", "d7dbcd410824730231d5a28a56dadf0776775888740efefba966e5de70124f370d70b8534568130514527498b8197e3df54d6a0c3160f17c8f3bd5c273c09620" },
                { "vi", "12438676a1dcaed0b9dc10754ba2852ad5e7833954105a120b3cf3d7a5d687a0d72417fc2436e77709c1455eb0de018eaed0699be35f3d572e85f132c1521d10" },
                { "xh", "5fb74ced8c4fc692e22614a87467a106ff63eaa74a978832095dbf6b61a0d785079b15cd3cc178c72e2b755113899525f42183dfa9d240f53ad69d1369804140" },
                { "zh-CN", "5e323537219392cf1ad435d501db3c22bd7b92a853426b52300c3898cab374796c6b7c4398f0fadd70a6637097769a249125988e823aae713d4f1cb552ad9bb6" },
                { "zh-TW", "6abb8439815245bb9257ff542fff8e7d7597f3b04c12d1fcb776059dca05d63028f6a27fc531010a651d4ffc8255b7f3a092219ff5886522c187dd31e88ceb00" }
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
        public static string determineNewestVersion()
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
