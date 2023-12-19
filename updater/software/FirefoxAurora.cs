﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private const string currentVersion = "122.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4fa3dafd0a73f9fe75bcc7626e76a857588acd3981e8c213ba7460df59d9a7c075469db1a85a7af25ccaf02c8f9ed800dca07f28507caefef09c9fadbfbfc20b" },
                { "af", "745b12e300cd9323cdca2d3d5da4eea71d427d3816d211b729e9d6fd50c8ffa809cec310d9c9f3be369b68ab06ec49430368fbcf15c04fbe64b4da54690a3c61" },
                { "an", "967fd314179c4120db555a261088dc17b1ca38186498cb08b5e1d975c57fd156046cf4e0bd39ee53a0da985d2f7e5cc51ae49eb90211c86750e672ee829d2b2a" },
                { "ar", "6438a830a711eb3a0983f7f59052db036d2a31fbc993b8976601530fb47db1e853ffcb713ebbfbe9a928056a39020ca60968be1453e7bb2dda467a7b46573c78" },
                { "ast", "cefd8d2d6fce5a18ef7674108be145867edb75c9c50b0211f67a357ad80bb0cb71b4fc330f0adca7aa6fe5c5a43537ddcab2852ba7448356159eb1a359d72039" },
                { "az", "c9de109b476b3b3c975f1a137eb8119b56df50c23be8e55e87968d743085dfa661339c5cb8d0e9c65f03fda6e078bc2eaff155a49da467709e10fb45cf2a7d59" },
                { "be", "c6db360d1e7552153c3bafc465dcbee4742b92e85d5b2cdd9224ca8d94d86f6560803b4a8cd10a0f034c9a40bb85b326f49abfe611e2b082f85aa47025432254" },
                { "bg", "56a048a8c49e0a7df0337e910b2337810ae34fba476699fd2f8ec8dc4456fe30772967eb322f022a129b17fa6d4ab98acfe0e974e089dc8830aca2e4bf5ae863" },
                { "bn", "e52a6b3bcab8ddfe06558bdff2108aeb9e7e29bed3e86d8f7ab1b99351ffc640bc3da8061bca44f00c84648fbab1eec1c966121ee543fd9eda7a071b8ebc8ec7" },
                { "br", "c68cdec980ee472b3ebe7ecf4a73522b90c449021fc68c75a9f6878d1b53b765fa4535bfd802509c756358ab8ccadb741ab0c2e59a151f301c90a9b8c0e882a2" },
                { "bs", "afdd63d8227425120b3b64406a4e6776d462ab93ef7900dd528bfdf66479bef086a28ed46de85603bdafbb40a9f0b1c90be11c5a925ea1aea24ffbe8172274b7" },
                { "ca", "b8c9b8640ec6a86047f3b37f02241c4e9dc9d9e0de163e4703a711f838321ff776538b9ca6f76ed2af9822386c3b180f839c0d6334d8e0e5baabd00e0520dccc" },
                { "cak", "57b3a9cfb04d219d3baac6fc07ad8ae5cd92b97a47a7fe0a290d138c20dae3681ba07cb7a03d9d4acbe1362e24c879d39c4ab5a48e078922df5e5d69ab433cb7" },
                { "cs", "a05be57ed8f9ca2e5832b5c17bfc09d2ac1328126eefd7073a9c775558c7fa9a0abaa87cf3af2c86daaf9446bb7640b93a8e54a6be2be163b31ee7041fb589b5" },
                { "cy", "3ff5c0be605796dc603bf5fb30c09ef87354df9d46c425ec75d6e0e8fee8f8868ae89bbdbfffd7d1d15e938a938e2ced955231434642ca5991578922005cfe83" },
                { "da", "0db0d247d88c8a909df7b3106d2e5a8503b17d78e0616f264e1e07f7f65459b66530b4907c3fec5d0f60c53cf6d74b7f60344a502fc8022a84a1cb58331bbfdf" },
                { "de", "145a55519f5280cb38b659cd0a1ff93a0095627043ccc62d6107d5a83f553eaf422aa03080e378aaca4b5458139e80623e0e7ecbc9d652f68247306cb1d7a81d" },
                { "dsb", "8ef0bd25492a46ed42a630f2701e5689c0f6954014dc5e976d1baf6cf7f2b300b7570657e1ec3f8124798bbdfbf983bd341e4c9b7adf924d4c9f0bcbc1c3fdc9" },
                { "el", "0a6cef18329e27588db239e25e1d628ce4fedbdbc31d4ea7c0d541cfc9924a8a491a1f3b6c44a87de61ec2dc769b360f1a93e888dcd55edded5217658b111330" },
                { "en-CA", "a29060e8c6d75228fa59efc550cb0ded8c79adbe9f23718ae9aea445f2bb5bfacb7fee317488cbc685a9e35c649d35ea27dbdaaa69123086c16da8a7bfd99580" },
                { "en-GB", "c1204a7072a9ca6a560a1ae421ed7b93d69428aed66bb2377d078b8ec2e5525a2ae31f98c6f95774dcb9fa778bd398a6e36008792ed749174617debab889cd12" },
                { "en-US", "9922e3db701e81a70cf76ce0599dfcf4899a048a0598ed84a15c25d9e48f80cd17099d25529381f9369362b13d632a6a93b1ccd586c93f1c3f85781754e7efb0" },
                { "eo", "93296e5d1730c36784a020d289325f905fd38152285496824d00f960a047dcf08d3b232862aa03852b30785768b3c648b14ce585f82989f2343e4fa5fa8b63e9" },
                { "es-AR", "668ed751f76a6c06b48380ef9a3ce09a286b64ee11b83ab2eca42974b7f694390169e0547536d61b93a655c78f64304c1b185e3c68f8f306f96b6b7ec1d14254" },
                { "es-CL", "d387b2c53a9b781b21b5d7420cd4f18bfe5934dd5fc37879cc67600ec4d9470798fdd9c34a5eed5aa0cf4058ce9092b393abc46fae5d24172d6d656f185cdbb6" },
                { "es-ES", "f30e6ffd737e5b23c9816496edd9a729d2643cdc5da63d91e034c00883bc0b99dee7f0ef6d668dffc67b6c5730e4d1f680df99274b0fa9d9289dd83b4a2bfff1" },
                { "es-MX", "6b167d81d3a3397cb4ef14315bccd1b52eb957317fe6d756606c1428a08132341e901b79e50407055a2af3119da6f854d72235041098e8e95b1fbd844383486a" },
                { "et", "05be6dabb557e9b03a6e620922f28b8d70ad7bfc7068d25bcb922636c8064beef51bb0fe6654561d252e276fca6d9192ff70784358ef1b8e50db8d975a395545" },
                { "eu", "990c9484ed3a59473829995b0cfccb0e861c73e5bd782f7c7743a549ea4bf38a7e22b550df79a5957aafa1ddc9c4c3443e2b009290ca08d92aa088cf5e180b92" },
                { "fa", "cb442f0ab35758350d6f68532c3fea03f684a7ff85cb1c0284fe7dbff3fd983ea5047c0b3190155a5496d41198db62f91de57f6caf7ef917f0ebf52ccfa4053b" },
                { "ff", "5b830544edd027a9935ef649909fa589cba902a06be4751e54a72feb19973d423d089bdf686ce8b82b43601caa8d49c7e20b43b40bce5736ba72b5d06da857c7" },
                { "fi", "84eb50d18fc20dbacc8cd85eb8618219b23e257b94f5d58c437aa0924c63c5da340220cf6de24b1e78e3e0ef9c39756ad9680bf5b84c6f246db5aa1800af6bfe" },
                { "fr", "c09ec5e7b6a8fbfc105b807e7f46664f0546d2e27e6a3db4a86e2805e3c0fec3c3d2240de26eea6ba02071cd54da5e5f187f0d9f7951d6d60275a466c42f779d" },
                { "fur", "bb43c628277f400d1e6c2a8a2eb830827eca59ee09a24fc4ef95528a73496cd6e7393ca774c5ba3125e4d5c5094366085388e4051735021f795ab547523edf89" },
                { "fy-NL", "0bf106326650b9032691f7cbbc2139f36c1a905e4477735ad574a8f0178bb32259a7b249d9c88b207a416aae1052c08aa2b05861fa5b951c81ffdedbc3afe23f" },
                { "ga-IE", "d209e45fe703de924c0d16bd758ee1691800fdf188ac51a5c9e645fa928786eabf8f0d82162129852c784a6471665f134c72f8d42c441bc9fa35c706000c12ab" },
                { "gd", "85e7da3c0651daacfb0c648a5306a58d877f63ba3aeca672cdbe5ef050841e16d2ddfddd9b9917e562998a27e41f0322a88389e09d4cd107b484808a60057fe3" },
                { "gl", "e54497416adebefdc3928043370f58e98e969183f6fb78d52f51417384143bf2f0767ef80069e63ea7146de775cbf60e6fda9a236b6fbe082db45d048f79804f" },
                { "gn", "6d1d21b2ddf5a899551ce821b56226ea0db56c9e9bb186e4bea18cad890c7f5c86d3e4e29e8213707510e76f9e0a9c781a4dc95192eb23af3039bda152f87eb7" },
                { "gu-IN", "e53f6a868f20158c2b8555f7152585c67006dfb1bc9a4b5fcc4681e1ed217a410cb219e1f2dbf1b661a48e132a9bb2d6e8a4d475081835f6d1ca9e68e555482a" },
                { "he", "1520c4824ba0227ae504386d3afb71190cc6386ad1e45ef71e83f6de32157217632b34cafbe082dca3a16dae511b9546751414dbe557b4c18fd756b6ff3c47dc" },
                { "hi-IN", "a4aff80941e8cc3a66071d95818564d0277baadfab9f3c2dc25009d0168febaf5dbc0fe3ca233cf99a6ef452b5d41b6f22a8e68e406e03a27b1113174eac62b7" },
                { "hr", "0858103e5257ce2324bf28d5ab5fe00d427d44b5e2b2b9336b7fa7c50af793a82df63c82293fa3673265ff6cc8b2a216fb64e3d5b2ba0535855848867d8903d1" },
                { "hsb", "7624fc11723a5b69d7e6ea7c53c8bd9f791e6fb20ef73d21e2f4141d74ee39f1c3d944b7bdfbd849cb47e6829092003e931bdf422ebbde3462dc72a59afde6a7" },
                { "hu", "46b23f4dbaf0ffd701d752f5f48ac2e741b4b6b6dc1eea9ef67796879a1f81beee4e00e48c9e4fe4bb4b5eedef14c4f6f9e0402318d88086dae4bbcfa8c1b73e" },
                { "hy-AM", "6736a44fc22d3e6a39201d0e36328cf280deccbc3df1e081c28273118c1b311c81ae511b72400f131c69c416b91a282dc6deda1ef7b2eaeca9c03ac0731d7bd4" },
                { "ia", "d49d843250a3cdde2eae115aad09f22a50f99269b567287a6c0ec3a49081fec6e2700b837292da919eda1a405cc8645e896100099bfa89afd18879ffcb060304" },
                { "id", "0ce3afa88475f6159248c6a9035fd2a1a7295a4f3274a13f01fa26c38f6dab5423b064826c44cdff90bc0dea3ba538fd9e89682b23fa4f45202b5a566a4c52e8" },
                { "is", "ad9eef2366877d35655b4f260c2860a7a4bda67b25d1eabb7619ea80085a24c30c3fae7cbc9b76a1a6079af87d343cfb0e5c1e6339f856ee3af41aa3361676c9" },
                { "it", "d19a20d1104ee1c05f5b2ca30d1cc4ce48fcdf7011bc5016d4a01d12f7f4d0cf7dc33a1cb5f7b07e53db20d41e5870e1b23ec828111ed9b8ffb3cd9ed728249d" },
                { "ja", "d47cbb1f863d5d2396d981b37d10982951d8f463865adedf030969cad8f36bf3e1536ec8557e990c0c680ea39e59f0fec62907da5c2600bcfdc991e6dfa99125" },
                { "ka", "cf364dd2f439af2db04dc21d6bb97b6a31eb6b49f7fe2f5d3c4b8c9a73dfa25e432b34cf369a0cbae968bca638ed0e164adfd42d21e996f145b292d97738e26c" },
                { "kab", "8b7109d614b98cbd5b9d56371f220d73735a2909f148946b041116b5ec3fdc29a481d88a74ce73853356e094fa2662a978e29e7db753bba41e0fe3c31614da2f" },
                { "kk", "aca13a397bfddedd8aca084a3b7982f4b3457e6cbb2f4cc804fb3ecde73302fd7a7badbd73b84278214b5db241230789d9f2076933ba2132cc0deb9616f31ea9" },
                { "km", "9b27ed3b7dea59941a5826709938bf7d46e63e44df87f01a04003ab3c4d5c85400e4b1f9c2e7e4c0edb9d1a4c9d71b54e92f070d1eb8ee6de2b1e0e6d6a98fe8" },
                { "kn", "8cdb68151717a35810ef9840c15d41217f71bc200ebd0ed2c5dd5737ce4f2fe76fb3abbf4248e8642fdf35e728e789f3d0b72b86cb036a1d128ae0feaa7b1032" },
                { "ko", "8129c4ecb98f8382ce3a792eb24660cf049438736c490bd5c69bfef6031693d3414556e4703ec1064318324c18a675c2aab5824db2783c416e4f6d3235fdfdd6" },
                { "lij", "8b681605ae0ec33043d43d1a05e426f565a84b2dcac518df988809022f037c2d0df3dd370d20860f6b59aa91e9144595d48bd61e762ebab2834d04778f8cb1a8" },
                { "lt", "931d024b36d8b706ed1f57a4da4fd26d168d1ec78993b7c99c170862f8793c525810ec239881fef897654fba2c8d206f0d7aae952e62287d19d064c9c244bdc5" },
                { "lv", "b1c419905746c5f0a53b30c08273703d7c792252ee7707a2a0987421b3aceda12493ce1f907adbf593e6f15ecdc0f15d168314f3ef5b00e60ac25c89d65e6fd8" },
                { "mk", "82595bb052532ea7cdce2c97cf5047d6da9351cae279c39c707da947d90a1c22370d855a7767f433b7c43c9bbcab7f50d1f13f795c11036f4e8ba6376a3a494c" },
                { "mr", "a55a9aba5eabc75b8329a39a7fbc9358cbf4dea15a698d6d4c29078fa640f384cf636c7dc96526aba4b2ce518e5e303785a907d48f66ef7f8754a712979fc3f7" },
                { "ms", "6cb3a721af1b52a15f2460256fb979c059ed278d4dcec61dc2d8e1fedac4ea5c9bffe863c6c9387d3c4c5dddd785d7ef0549641137792a94c39352bb9e987112" },
                { "my", "f0da8c71da11c22dd7e47e8ba7bde83e9e3cce02d275b241e32ebbe9e3ea174867b48728e69ffad924534edde19078d5ca92137c05a711be47704e2ffa9a6f26" },
                { "nb-NO", "b7c0b6c6b8bf9255876acf7ecef4b63f4184aef1dae79a4bd38f90c375830bc29c2d3246010e59eb2610b4e6a44880feecf9c3255034e295d0b7e9442c51d04d" },
                { "ne-NP", "2cb4ec4515fb60ce90ba5a0acb3944fed515e31a86a1997c03e33ff369fd15b4122c777841fca9b3835982b10dd2b91e10c452bf004d4a5eaf35ac9a0a6f474b" },
                { "nl", "d9ba746cf86fac5521ef9f211527d9fc84eceee371c37247b7c265c954588a72372189a576beac1d4e27479a6502f876e0d95d3b55e51e8449b96c8708406826" },
                { "nn-NO", "97b2550b174fc4665720f228198c882bf87f2169d5068764d14f3b498de78ebff48fc9bd9b36ef923fa33f536a5f4b2f44fcc8b8302a6287a3aec62383a0fc42" },
                { "oc", "d48972bc756f2328fc0272c044e3431c9ed3789965f886f61930ebc1e17ddb0c234b4c1c3dfd5727a26a423a337d98d34288eabcee90d595da386816569320c4" },
                { "pa-IN", "9f35794caf1f9a19cd756f8c5b2d6968d0338c3a7d1a66c069094218cda8492ce5d747cf62e88f009f8dbf3755af1924042fabb32b5f22fd27e410a82197a918" },
                { "pl", "1fdadf561e784123ad9da3f5cd790b95fabcef79725ec9d9483a292db68f26f00ff7a23015ba92edc05ad48c7a424f5a57b213974687b2a562f5198c56902380" },
                { "pt-BR", "12da6c22f2515f3c7a5fcec8721d3fa5e4c6ee420e3bb10c551f254a2ab3c0505bbef8903cfac027974756922b2d8da950c5ebd40d557a1d5535d6bbcd51de4a" },
                { "pt-PT", "d9e3a520d35908709afabe269729e53870a2f76c7c6ab511c444f8bcd061c23cdd221503e85da5fa8ae75a4a803ceec8b3ca95d65723dfdc7f78d6dbf6af4de9" },
                { "rm", "cc3ff8de08ac4a82e614a42e9527ac0abc48417ef9738c7760ca42d7a64b73bd24c7ae8af8f518b3bfa63f9e00678422dc7346ab35e05ac6297c13107aac7d12" },
                { "ro", "5786b53289276cdfa39e92ff6b3b3fc86188faeed123f599d8502cf040a0f2d763baf9d2e230e3ffe80d01de4505318a6296bc405a0d2c7b25a190ff7ea41797" },
                { "ru", "24cf3eeebf78e3915cfad7576e40f5a3eda630bf84229c07018e3e2d0fdf24f043b89f040ee3ab18c43e10ce418467560d3a5cd1ed1f2f0cba6ce52b2ab8e3c4" },
                { "sat", "b4327022daa37055942d9634e1c855db9b9571ccaaa795d459b87f1731b01290198b1e83b0bf541ecc519a597ac2a78d926c25ddaef9740a9ba01fa56378d792" },
                { "sc", "a0873ca7d066f347dfab4224236240aedb8f9dff4d844847c4edb0c358eec7ca0928db02694376f97a6260cb5c6072abebcc587e0065e5dcc296702902ee53c0" },
                { "sco", "ee4291e6be1f798ed98779f73a75ce307d3e52f3ab14828bb91d2ceed531a90f1b7f16a56ce0cdff7f0f52719c7c64c01b9648c3245436b0b826f446bb6ba7e2" },
                { "si", "890750624e9a4689918bd1f23988befeaadccb24eb0aaf742152930ee63b4998f38976a2c401e66a6a0b80aae0e522dccf69700851caffe1a22a96f760e967d1" },
                { "sk", "ba07988c105f4d229e01f49b54bb06575b18c1d6f8d716591be194bd739fe28350d07f79d7407cff7314007b3b5365da85b198d4436a61a5911df2226e14b9cc" },
                { "sl", "5ae047e05850e4105dd469637a3144bf838e79b992c8b6739af3ca38e86e719975460de1c30a0dd9311bfda47e20895fc23832057c3e139c41cf17a406dd8c6e" },
                { "son", "557aa053dd3693812768d69a2fe3a07bd6c6f9831b24883db7c64c1c88cee89f29c845857c1114445f4e3865e2b29797a30716748367e9f6cc64122df6ef35ac" },
                { "sq", "3aee1d19be65dcaf1718dd1a8457dca0ad9f357cbbd94a726612d10785f092677059852d6d519e1be06fde71217d3594f547c622e41ac6835644b34829f1e2ae" },
                { "sr", "71282600d8fd73230291e7e9c0eda4a320f62bdabbf3c101317786b416d45cfcea06bb0eddd0d3bb8fc74278d0916e69f4dac06b5a995c4cc846dca3faeb9866" },
                { "sv-SE", "4991ff15eb1c12e647046d03ed92cd77e12c1ee3071310af8ffcb280793b19e12c04c3f3d973fbc849595a622cae715933adfb7c1982627e6bd58f06197f9c81" },
                { "szl", "cf898ba914392458c1247ab3ba874a0bd504a078fc60c265f180f407272299a9ccb808904c473708d8008d3d7f3e945d09029102e18ddc7f66fe3229d0f4a040" },
                { "ta", "eade0ced97bea30df7552f43e2702381b586daa5abfd080803a3130df9bdc520aa54ff91316e9d0d84facf1b41cb09ad4c547e143fc2b604a3f4c6fef832b821" },
                { "te", "d8d1a533997436041b8cd79fb8089200b5aad07c63f81cc1077a89dd315af9e4bd7dcf4e2d2b48b5b1a311a9a884b7a50850daa071202002a71a247c9112d36e" },
                { "tg", "fa40384a04bddd88f64122ec8daf19ff67f1e3144fa647ccfd34c501be86ed9cb6701b58799bbd87569d1a1161bec8048fe3f7c91d266ace0c5c093d648f9a34" },
                { "th", "350fbd77a7680aa352a09d75abfcb2af1698decd1875ef3d3d032a94b3e27c075fc27c8d96add9a1426666cbf6aa2e7d03f139efa3049c7703e26986f7cbde8f" },
                { "tl", "63ecf8e4e55b22c137878864b6cdcfdaa425d10725828a7b6e0c5e33408008e3bc0c598cca483e92f1ece787b66df1818b2851f94418f61a228d564463f6bdbc" },
                { "tr", "0a54e94c33724a5be305a651c4072c0643faf86df0cf1f9d88cb53b85197f39513ae47905fc32faa6a5c0ac9b4bfb10cbd62d44ab15967a5f065851af0dda66a" },
                { "trs", "ae2e7451b5576889e0a9ea849f206d133913c79f57ca85cd48c5cd7a7516ff085bea5949e6d67399d79dda755f626d559e66faec71f169bf8615e2ea6b94b4a5" },
                { "uk", "315e022a0cde23a74f5335b237a56350ff6398afa832f94c702325da9e164c6f5b1f5024a0c73094fb9cba6745596c125c6521047d55a3c915bbce4825bb87e4" },
                { "ur", "30d4827a729723d4350ce324d61c0bfad397e6736c0e9d6cc3d2b2f95c33d83d1a5f0caec11e930a9c27b5caa3eb43e00c5a1a6fa566ab7e74dd1dca5b821613" },
                { "uz", "9d12592bda499e82b380ba249222d5d7711d30cd98309998634818917e33705fed80070774654024cc29d72493115d7f972a21b59a7e1ee046c63a007092b392" },
                { "vi", "107c4d9e245871eeb5bffe44251c65fc7fbab50a9f39b812dd6e0e3f07ab592807b97105278c54d43329833c02e5ee8577713a6fa403a2b59d1ba1b4a917f894" },
                { "xh", "0afe4dace83df14e01b442e33a51a17b35e1c1d0903c2c0ecc31a8cff26be7c5e46bc27e63d4734e60421c2a954fb075c294599ce9be30915e3ed322749b672a" },
                { "zh-CN", "17cdb8a498c0b18a257685c8bf6e370e530e22dc2afd8c9235ec573a5deee413d556bd6d2504a81b1670331e44787caaa24bb9604d96f3dd806e0a3facb6e7d2" },
                { "zh-TW", "49a000a415d2bdf97b17c26a326119711d646aa9ddbc189a861cdd98c9be139aa18f69f896c83503796efcebf45b234f461edca3b48d0928c6faa57bd8081b37" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f479ac55709f3df611e8a1387b1cb6bdab4f87e61d0eb475d944e41548969181a2a70d19c45d95356695443491920b2ae04efb5737fafc050c34465131a4c44a" },
                { "af", "c533c264c3ced66cb866370c3340c4a920f30a5657507ba83fff0a6700c1d91df10e5f83d9d2a03b75f901655ec8ff7f4b49d53af89977b203be7f06bad2283d" },
                { "an", "5d9d0d306149224de6b20ea26d2432a4c27c739361a28d0e34b13ebf337200d5e13d316881414e8eace59965c81e45f5c485f96c486554c00a8b672a88b81451" },
                { "ar", "f345c47eb81e7517c42e9e578e51265c8852c80d4b452d78592096c98e1c5291459036c785ee726e6e430b77c9e2dd69945bd838e104c59ec59a99c2d5cfe446" },
                { "ast", "a2187ec792c9b68695b79d13207b2f43271afc74f133565168e2c586e3e5beb4e66c04dbeb0d95c9a42e325702a27a28fe7c50c6d742cc94c0c40fde5754f271" },
                { "az", "a1632c7926a8b458dfe76b847bd1336ad848439357e343ffa9c0ff4a096d99fdca999d71dfa65cdb4f3016b86bcdb4b7518d74bb9d852f2f2d4a522b61ea4b09" },
                { "be", "f460d9b84d95e4cf630b5a9ab821d88baafd9a87282bfbc78ad6663997ccaa2e56332c8757334bc547a3a2d0c0e902cc1f31f23fb935ffe00d708da25f333688" },
                { "bg", "9a4d54d59ef429fdcc5c902355ee0156770d376f6ca51c002c9871403be04845578dd5dca05b1da02c34400e78272a3db57385fce0802489e6c0691d5eae27af" },
                { "bn", "af6ba3df4ab3117fc834b49d17d9fc91bc0e93048e4ba3d1ff53545e4ad010f1252bb1f21087212ab66f80b99ba820c96f460c3fafb70f56f37bb73e6f8da67c" },
                { "br", "226f2be8ca4498c8485d138fb44a979f03aa93858f0c250f1b16b082e3696b22a093cb7889f36f007b1bbc08e6952758b2a665cf9ffeaa6cb6e1ab66129b1e98" },
                { "bs", "80f8c289df483a1cdb5277b9944169735bfb8a792ea1172e77e8158bed51f21c01027a8def77a314657dab90fbeeb581a68089e5e36467f998fbd5b52055439d" },
                { "ca", "18ade772ed18b023ceb35053fe0105c02cc228565ee136335c24922f2de3e755753028eea57d915303bce2a2e8a0b485fedbf8b0c49706833c38687dae77befe" },
                { "cak", "9dd5f444638090dc9b50e2cc463378361ee825a0b6b75de89cba93a572464d13088be3e0108a3c19848c1da6da87082e2f2c9487efc8cced83b5af4a29d5841f" },
                { "cs", "41963d32f4bbc5e4c55ebcdb25c2f0efb219e48ddf0c214ddf64393b80bed9aa656fb15b285f3c12606c688cdb863c8fbb19adb689d6a56a27bf82c9ee1d329c" },
                { "cy", "be5454047e99af59cc185416b259dc6ee6d48d5b1188734d382f40acf0da3e9f1e22a1e9a3840c580c197c0e3464dd2d88c4fd23cac397d0652e2d18d1dc4f4c" },
                { "da", "ad36cf56acbc4c19c293381c4427121ca3d55a0fefd5ede7ae6f90c90b3a89387529119a4a233e8ce92ea46b2a313c518ac4ff8f5ace1904d50780db603301a4" },
                { "de", "4f40091092af5cb5aacdeb65ab03600afb52c334d2ba57dc258a7ad16a0119f08fe0600f33cb00fd22ab180b37bad45bbb49cb0b634205f865ac1e6dc69a4bea" },
                { "dsb", "01a6f46d79b0bd1a7489454cfe461bcbc2e17f5c6544ffef580fe6fc2ab944d910043dd1275c37b7910963cb0be10b14503819be135ad13bb3d9a5c5b42042eb" },
                { "el", "59c15a9f60dcbea30a21870f2631269f0f7b6204cef66561064e7c66e1dc792664c9e07934d923b518a267d41ed77adc147e3c6d98658bb79b8ae925e49d2190" },
                { "en-CA", "24ae6969f294355ef4e9165eac8a8c9c15bc3f40d7b1742ed9a8a447a362ce4101a1637c09cb9be76aca235b605f3e678e58de952aa3fa7dc1108d535924027d" },
                { "en-GB", "865a6abc5ebc2f1206c95430ff3ca4b257c5004e540c72e6f06a689418351cecd666d0518eb6eb4866a79ca78abbb2dc325d875bab16183061eb3cbcc620fc0d" },
                { "en-US", "120e00b677cae0391ce414083606153c420082e4d36f0df929fa52b65ed07cb141466e11e9bcecb40f7b12e77311c777dd6d330ab764355aa6b32c7972bdc009" },
                { "eo", "bbff0e16a117c84ed8d394f6c0d62b7324bf7660086dc8196ecc52df2b26ce4c1cbb575d6a32303a4c414dd61debf4a978c93219afa4b03cac956fd88bfeb5cb" },
                { "es-AR", "d7bbc2b27262d66e54b3d2b12c1c85b6757cda6f549afa0539ccfce6813763d3ea5f3b820abee1ef465befe543d721d54ad3000efc7b3aca0976b0849ecc0b70" },
                { "es-CL", "31e317979d35d6f82c9b4df2ebd4f91e50481c4599d7079c506ea58af840a4e6e3d3e64e6e6b7ed5c5c0374113b8863924785e5d83730e1583e47defc325b31a" },
                { "es-ES", "c1d8e12eaaeb1078821e1f46c78ef0e07b8f591ad224fac071e4ab53d3e9055f681f3462077f4bc54d75e9783cf33556a241175f047674ec47ba6a85fa41b79b" },
                { "es-MX", "d7d51c67951fed368e57b8ded5625cc16798b2cd3d96067c75929c40782ba0f1ac6a9d6bf5ca2a1ae1ddb33b9470107cb0ed9a22ba961451a7e2b91103c821fc" },
                { "et", "b8fdaf5038857e4e3b3f9a50f7315559607a611b2ac071ba4f76dfdf87885c72c634e4fe04ae414ca8e06ec9e6541fa72a1e86894d88ee76d53d7c52098d5643" },
                { "eu", "862ff97f03bd60de8012ebe7a4c6ee29aac72bf8b2020066c1d1c09952d547c3a7d09a3a890f5ffec9a451d0755c4987c1977f8d68b8de877cd4eedc6fab98d4" },
                { "fa", "e491ee5b3d76ea9944f6ab944924ec0be448bcb5fa0862b4d3231a00f50109d7d0c157c10f65ca1c9440e02534e1761cb93f8470cf497dfacf5a1dc58951b79a" },
                { "ff", "477d5835d906a0781ae54ec07daa28cde76ddcc4b5f1ae64620b55239a3a4ed25656887c6921008de84e40e3a21d7e7280da7e97519b19650acf8b73503d7fd6" },
                { "fi", "bb426f7b554c9b4c073ee13cf213a62dcc23c495ae597f39da3056aa091e4082344f428e902a1926b3897d8d6ce811e342e7cf3be7134eff156affdcd66622a2" },
                { "fr", "6c8330d9f5d29f07db3977424ac20210330baaee402e97922cadddb364b9ae962e7b28f204654768531b93a7029b10cbc41d79fdd0931f85453a63df75cda878" },
                { "fur", "215192841ad9f58940c141436b086f2150f4c7f9cc025ce2056704df741aa26592fa697036ab1fc6a9edfccb343deab6e5cc3072b725343c02e35e13b9f28602" },
                { "fy-NL", "0f6a9df2b921800e5d8ea9ce0c97db51d8e604354fec9bc47542254c940fba5859a1c1742baa4fac1ad70f593d552767e46fcccc061388c4a0912015cae63daa" },
                { "ga-IE", "561336e25f43baab39336d86d3e70058b85181603f3fae1f0e29e52a97ec388a236d7e40f855b4acebaf83418b4d01d8ce72fc418bb0d578cd269eb04d787d63" },
                { "gd", "6ebbd04a1238c9a251f24a6e6802987dfbb36bedf8eb54b3e945a964ddb3f7f3f72d03aab5cca29f21d8c59efd76056cb842de08db7a0cd81af5ba956345051b" },
                { "gl", "cc40fd3b5020a5826e44ddbbff23c99be238a2aa7ca8a7d490639febc11382d5fad402d622d899022bd6e68ab431af1aeb389895926364be9aa281b02567236d" },
                { "gn", "c173ec2f1825855888327995c1c38b19d21856d32a006c55e2f33f94be86ced5dfadf8db397fbd6469755e40746dae045007c5324bdef561b8a5783184e9f8e7" },
                { "gu-IN", "78261b79a4b424d9d3720a17e272d086a2702fcb31ae35526bcd822bdb8618b8e49f8cf86799891e40f8e28cd249bff9447f857f32ed56adfddc4c435be3f432" },
                { "he", "b2f84d737a78d7fac4a028a56a84fe9c0f0af43f58f7ce89b0a97a17d0c38467e0cae0e887b5378a68e05c9ae1bbee57fdc70fc32b09b9ea8417224ba719f20e" },
                { "hi-IN", "87fe8a2ebca065971ec2d7ee351f126618bb22c7252daa1979d36dbe1f22441986455ab8aa062ba80e4b10062369088784020aff97efcc07c7ec4e0bb421b9ab" },
                { "hr", "62fd657ca0277b578e5829b1bcc93ab87a432e456977a1b0158b40a042cb8548f70f45921f9c5079cc08345991dfebbe6eb3c242a4b74326db8c8cf5c93680a4" },
                { "hsb", "f943a38c867549176333ce52471757a267b4644317550c04bd0123cc73157d65bcb9a53a8239c862966ed719bce08105cd968c99049f42a44dba98d6cfe1f188" },
                { "hu", "9c60b073c0762d87b7ce71198eda38935f72822c415b06b301fda73734356ccff2ff4793f97cb460a95960c096821ea3f354f18a1956c2024aceb02df0207c13" },
                { "hy-AM", "6b890ff0074526328519345f31e315f30b615d4eaae9cb56a3fc2861c1a93b30826beb8ddd0af6db07a1de85daa569f1a8a0e75e63a9e992a67e39655bb019d7" },
                { "ia", "0e130b4f7ef077b0f0b85401c3b7ed5fef5923c58131038f727dd5d7756dcce4e2456be87052cf690e988bf979a14e8a8b57ab807a004f8280f73381baab5693" },
                { "id", "9b02e6e9eff0fe1d1985f38864de891117893f3b2a6818745b0cbb9c4c5175645d8136a5dfd599565f0dedb96206eaffcd98bb689edd5cd5cd188847a5850b36" },
                { "is", "1fe6a11874c8a17f97417723744af34d2694121f38ac72043ffe3f4741098cc14ab930e93f355d24187966a7e4635a2597c6304558c5e3b50f728967098d2b2b" },
                { "it", "53dcbca3c371fdbcc99c53a0b7bfa4a100ac17f999a2941baa751dca2b86484af72c5cc5fb4a21165a459693ee1b5504603ad7f8b32ae6705dec39b169b730f1" },
                { "ja", "e4516fd92d303d3a9fe57d6f868167ea72f121fe2c606a2fbf7ae927456ae1255f6d450f860f964bcded192e56c00c842b1e61725bf0106e8a415d411fc09e8b" },
                { "ka", "2b920a970bbe3b00a0adee3cc549c79d59e92321c5d520b290b6542ea905b334becf8dc242cd0e74ab5f3f2078558fc533e4b153b94f8856f9897aae11de4286" },
                { "kab", "af6bd021c5bebcc14f02d25f1bb1a0a1ededf175426678478df5e32ce5292b9acb881fb53fe520541de587252ebe37f609f2bb4d8278f25e56cb993264c8b308" },
                { "kk", "0e4335f05d019b2cc12bba178e091830860021711b80d018f51bad1a8d546d7145ad170608e089bbcd7141d9e46d85fc5cb6b79cdc7babc5a3d466175029dd81" },
                { "km", "5aaf1d3ffefca8784c610217f98376485c6e1f19ffcf7985ce78a8d75ffbdcfcce8a8fc3c1ed2f90056621254dabb73b2435d0729f68680e532d3fe3804d432a" },
                { "kn", "dfa67485ccf247a48ebcb6cc35060a28f1c3072244e77b94876a12f0a98e8e0efe8484ed0e7602266cfa38e796f329c133f9481112aeb68ca85375a3d5f9bee1" },
                { "ko", "12b4209081ebc1a9604d21ac8b37551af1f27f3736be9088f4212a529c61f09fb6837cf8daf9797db4f7c3ee2f54ae0059f745b32a980d47035fe97006fe1548" },
                { "lij", "704338f0aa9cd3618e5d222da73c60b8682cb22c82f830ff2e2052b9ad8a633d5d69d5592b02a3276cf7daa0960702a9f231b5d94ef5185afaa426f1a3f75e9d" },
                { "lt", "0e527994421e2913721bd50a35dc9b9e38bd7f192067bfc008af13b2b11e98b0517bd244a6c1889b3956572785e706fb0a05c2c7c05bc137652b4175a3083895" },
                { "lv", "90d37535889ef8a52b248af0c6685e5f0ff8d41c7146e13fbaaa00437f4b6146cdf92a21ce449c1c959f668f911713bf082920029df0fcc7e0c6e548e1ce1a66" },
                { "mk", "c98e6df9478a4eeb2dd105cb81d217d35ff30931fec91cc93644f493f5667cec8a4e89fc9a834bf9d1dde3a5c47e248ed90d855d6291cb20d70108b43993b8b2" },
                { "mr", "960b2c0f76ac8baf5f6afe701602a1f181402846a663160e3d3393df9d75bff96c192d4b65de086431acffcb3e7b0b47b6536ad0bc89b1244ab596328ecdd2c8" },
                { "ms", "061ec7aff3c54f13081b340e9133d28b18ee4190783b93ae7b3f6c7d6952653322a8a07a64f8e4f57052b215c8ee489a92ccccae026f1f9d62718fb725b08c79" },
                { "my", "f08bd63a03f7e594cba3b94807c12b5302965fb97a4ba0734cee6304ebb3a5ce6b82fb2c6d7f86515884141975f7e84eef06d7409df5bd9b0738501a3aa185fc" },
                { "nb-NO", "ffbb5e77f80c11163461bbc447a58d5ad91478b46a3705fb3513e82f4d059d4931ab937066e665e328455234688acb4bdbc4ef437315c701dc071083f29a228b" },
                { "ne-NP", "a9698f9eff6816fc10ec2f17319f6d9a1b6db4dd8b3cc5f9bf71202f2586309a6842fae405223fd198954212b31b1031359fe29ce5598b58950cd2224e95aa46" },
                { "nl", "9e0377074903467fc21c9c43c5ce371adcaed88be4dee3b0cd0c713ab2316398d0826eda78e86cb1b8506241780df2022834558beb2d23215df7294744cc0dfb" },
                { "nn-NO", "ca062f4e5ecf36151d3bb2c13380fc9c535dcebae84d0bf3512854dd389d2696280d1fb3a4ad3caf87475162a9477b0428e8e9cdbb9d3b9597a76dbca21ff9f2" },
                { "oc", "1e2e5f73217c18f47adcff8c4fd1124e3b68e43ed632cee3a105763dc7e149afa7baac4e18f9ad9052ebce7dc1284a1d5a2d1b9c1d5ea424fb331d33cebfa7a1" },
                { "pa-IN", "a2b42b1add630ab5d2d82c6cc8c93cd02981d8bd890b7c50ec9d52e9ce6bb452346edcd070a8a9e5d2b8c6fb63245b395bf12d4eae8133556649e6eed34dadce" },
                { "pl", "508b2f703cc2aa0352ce0d4eb0bc9e98496242d4c86fae7739a6219c9817c4bfc4e2ae2cf67a0f65e233ff97939cb7a425f60fdb313e787cb8b5b77aaf5dab0b" },
                { "pt-BR", "610b5b5bb2f2022dd0978d6fe56ffe5e45cc4e54b911a36baab6cb2456ea6204ff121de338e24a2e2a46a9751daf6683ae55646750f2d8d9b2e7d14f32425cc9" },
                { "pt-PT", "9cebedfaad7dd4cb40a9fd43e6a275dabdeda2f2ca577da0c95d2f2763ee959be94e95d5842529e1161fd2fcb517e1ea440d470be464168be7924dbf1217d99e" },
                { "rm", "5038e24381494ca3a6fa731f8cf5a9234030b900d38b73cded4a6a7aaed1b26638385ae35103c6506c19890aeac386f24bc577d1d48f932992e232401b15f7f8" },
                { "ro", "8a6100c517881568f0b95dbd0ad9b3f0d4b79493c73cf0192e6e2fc93e15f9ae4ce863658860447c0bcc58777c479f461797802d252b9c41250ff1b90a77de61" },
                { "ru", "ad86590ec0fb0d1031040e313e644d78416988d2a13760d81195b79339b392edb929c2b8a37e86ed2801beb8c6f9e144142b837b0b7af2fa440ac58bc5de258f" },
                { "sat", "5161b0b8dc0e88a6223d1bb9a7dde15c4aebca9ec315a9edca0f1d05d1b2921aa325162cf86c704f8f3ffb7abe9b7745bea19294163c302b00a8e37380bf81be" },
                { "sc", "a329d4320034063a290853ac6b1c878ad2c9db7c0411b98de3e592c250c2acf7535dfaacaa6a04c3e1aad9b5b941124cb8c1511a40887efaea4bc6f912f4fa7e" },
                { "sco", "fb3ab257dde71f55dd0c1cb84f36622475e84f07f4a954983cd8d9638de7ee1351007a8e2b16d19de03e06f871442863f88dcd2a5eeab50f68a8822da19345cd" },
                { "si", "e7cb91a24b63fb406e19706783cb0bbb65528e5d8849613e721311a87ad2e7f27f8326be0efd98d45fba9b2cdcb094bc3afc4531817139ebcb2615ccf00736d0" },
                { "sk", "dd8b66052d48ae39e8618f8c5fc2490eee9f873df062150435e4479d6ec26de80defd680d6dfe36c0ce84a2f5a9a2cefb5abfc1ab2c56e97291cb496e264b2b6" },
                { "sl", "ac3c5491e85b77bd5bdda5d474d8bc22c846a01807156706553a9ff768846f681af9924d85ce2666c553e296235339b906bf6e1cde842b1e1d79c7f92697c11f" },
                { "son", "7cb525228772e32b7e2b3ce6bf2cc66dcac9d4abd423ffbcf524f7fe0b71ec03811cd427137649f4d2bd380c545d1e5714c180b161b4df4afdf3c8209882e9cd" },
                { "sq", "8964afb5f51bdc046b015ef9a49ea8de3317209d9e30d4273bff87fcbeebcecce9868ed6cc2ee68932f8f25cd38692ebb1fb1aa97216bf6c57b34c82c263a884" },
                { "sr", "539fd1629f2be1c100cd32170dff857dc0b4d3778f209f208e77267716ac898132afe27e3327403fb4532242b4283e6ba212648b9fba48691db05e9c50fbc5bf" },
                { "sv-SE", "e3dd01f4854eed768f00d509e567c3b208e4ac1e37de057e2f2d879e1884456fd4847beab8435ebfaf7f777f0c00767aa6480bc0e9075896d838aad4a5a0ed45" },
                { "szl", "8d18df8c7e22ee67f11dfaa6a50383a776096182994e57565f06b8d17982c23804a25ba6a671d478c5d4b468e3c10d70d062c43408a6068ec7bf9321609434ce" },
                { "ta", "21bf172bc622f143c75f4e330dd9f9107bca4eabcf9f8ee7a7255fa2836bee5b54e19ce92963bbfdaa790ae18f79fe72a80e3545c47188aa0c228f58f0609be7" },
                { "te", "81cbfba05b35856b2601f0248ebeae38b267556bf529eabe4e0097da3414cb750035d0b9d61e7ff10317cfe9edf50b0d83f5eef2589760813507ad6822666045" },
                { "tg", "25b8853b7945eea6ee1117dfc0b8eeb8dcf0704c0dd2663d0460ae038210026bfa4472a8182134092cc5bcf34e5b6ec651f58c167f4f49552e1a4905b3e2f69c" },
                { "th", "6a7012c462a597da68784600ad86dfac0ac0ca49570d855e5e81b330ca3e3f5b6dd5496c8514524f67f797b2e1f5c638c40cd5a859f54990308d93d782451e34" },
                { "tl", "918e6511d60dd20f66f4afe02818037f9fa11cbaf33ea682a4ca00a079b834f3ebcd3a8767b041280fb0b4f96e74862440439bdb14af98a1abfc9bd66d1238ec" },
                { "tr", "d48db6434e7909180364939d45b3219238d6a88717c95e9af5a0a54ee763a433f7d2c590e32446ee7779f752084c651709a0f0fbc0df079b1d771ab8af6fd5e3" },
                { "trs", "aa0578c5f3e8f219b3f4763afaf05468f44cdaad429b76ea530dba729b654f73dcde40b4346f9391e60b847c6f38c950cc162196c1487dae04084351e30a6294" },
                { "uk", "e4f7fe292ff9cab9b79e5117288445eed7c1143c225b13bbfd5aa6690cacd70a478e8f851ff6f72afd0e682c0f8f758c0d6daab4ced34cc68404500e0cfe0e4c" },
                { "ur", "399b71ea4f5b234376be62e8026f7ec4309b4ec3fde2ae84486e8337c0c43480ccfc2d44a179e41b4e5c2fdce126b40246d0377c11181b18e99f148446218e91" },
                { "uz", "298b20236ed7178a954dfc9ba04ac98f80a5696e15ff18a9e9d9992078f6d1ed285664819a6f70f890836c5b7043ecedd7739ee5e83e9fb2b48dc853803d2c6c" },
                { "vi", "21c46b00509a11b71971e592747dcc5b259735053f73bd0adf7f308233ad0ae3ed0114948fafe7f681b9df5a87f6457ae96b9d75e1fba59c077cf729a6638594" },
                { "xh", "a9741ecde7e9074afbd8a799e443ca56251b5120b29a89f263195d535c3eda50c9572a7444c8555d8f0119f0bb3c0487f908cb1b553f281b13d5a616f2eaf261" },
                { "zh-CN", "41c7eb3e9a183f2659388adf86a4dfce0bc50eaff2898a87066a5faffd13a219ee4a04e4c32dab7b8ab3e0d46b3c13945045000216210d77bcb7fff3952474f7" },
                { "zh-TW", "65c1bb529aea48f82dea62d09e1f260c50c9e80fdc7e4055c5f5ac4abf6030f2980737146ddad07968da7e2615881bda47378adb77b2f5c7c2ac91280d1755b7" }
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
